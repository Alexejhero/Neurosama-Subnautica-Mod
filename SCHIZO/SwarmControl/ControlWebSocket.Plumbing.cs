using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SCHIZO.Helpers;
using SCHIZO.Commands.Input;
using SCHIZO.SwarmControl.Models.Game.Messages;

namespace SCHIZO.SwarmControl;

#nullable enable
internal partial class ControlWebSocket
{
    private readonly ConcurrentQueue<GameMessage> _sendQueue = [];
    private DateTime _lastPong;

    private async Task ReceiveThread()
    {
        while (IsConnected)
        {
            string? json = await ReceiveStringAsync();

            if (json is null) // close
            {
                LOGGER.LogDebug("Received nothing, closing");
                await _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, default);
                OnClose?.Invoke(_socket.CloseStatus!.Value, _socket.CloseStatusDescription);
                return;
            }

            BackendMessage? message = null;
            try
            {
                message = ConvertBackendMessage(json);
                _lastPong = DateTime.UtcNow;
            }
            catch (InvalidDataException e)
            {
                LOGGER.LogError($"Websocket recv message parse error: {e.Message} (json was {json})");
                OnError?.Invoke(e);
            }
            if (message is { })
            {
                if (message.MessageType != MessageType.Pong)
                {
                    LOGGER.LogDebug($"Received JSON {json}");
                    LOGGER.LogInfo($"Received {message.MessageType} {message.Guid}");
                }
                OnMessage?.Invoke(message);
            }
        }
    }

    private async Task SendThread()
    {
        while (IsConnected)
        {
            if (_sendQueue.TryDequeue(out GameMessage msg))
            {
                try
                {
                    await SendStringInternal(JsonConvert.SerializeObject(msg, Formatting.None), msg.MessageType != MessageType.Ping);
                    if (msg.MessageType != MessageType.Ping)
                        LOGGER.LogInfo($"Sent {msg.MessageType} ({msg.Guid})");
                }
                catch (Exception e)
                {
                    LOGGER.LogError($"Error sending {msg.GetType().Name} ({msg.Guid})");
                    if (msg.MessageType != MessageType.Ping)
                    {
                        LOGGER.LogWarning($"Re-queueing {msg.GetType().Name} ({msg.Guid})");
                        _sendQueue.Enqueue(msg);
                    }
                    if (e is WebSocketException)
                    {
                        _socket.Dispose();
                        OnClose?.Invoke((WebSocketCloseStatus)1006, null);
                    }
                    else
                    {
                        OnError?.Invoke(e);
                    }
                }
            }
            else
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200));
            }
        }
    }

    private async Task PingThread()
    {
        while (IsConnected)
        {
            await Task.Delay(1000);
            if (_sendQueue.IsEmpty)
            {
                SendMessage(new PingMessage());
                await Task.Delay(1000);
            }
            if (_lastPong.AddSeconds(5) < DateTime.UtcNow)
            {
                _socket.Dispose();
                OnClose?.Invoke(WebSocketCloseStatus.EndpointUnavailable, null);
            }
        }
    }

    private async Task SendStringInternal(string message, bool print = true, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentNullException(nameof(message));

        if (!CheckOpen())
            throw new InvalidOperationException("Socket not connected");

        if (print)
            LOGGER.LogDebug($"SEND:\n{message}");
        await _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, ct);
    }

    public void SendMessage(GameMessage message)
    {
        _sendQueue.Enqueue(message);
        if (message.MessageType != MessageType.Ping)
            LOGGER.LogDebug($"Enqueued {message.MessageType} ({message.Guid})");
    }

    // note: cancelling in ReceiveAsync kills the socket, there's an issue on this that's closed as "by design" Smile
    private async Task<string?> ReceiveStringAsync(CancellationToken killSocketCt = default)
    {
        if (!CheckOpen())
            return null;

        using MemoryStream ms = new();
        using RentedArray<byte> rent = new(1024, true);

        ArraySegment<byte> buffer = new(rent.Array);
        WebSocketReceiveResult? result = null;
        // int totalBytes = 0;
        while (result is not { EndOfMessage: true })
        {
            result = await _socket.ReceiveAsync(buffer, killSocketCt);
            ms.Write(buffer.Array, 0, result.Count);
            // totalBytes += result.Count;
        }
        // LOGGER.LogDebug($"Received {result.MessageType} ({totalBytes} bytes)");
        switch (result.MessageType)
        {
            case WebSocketMessageType.Close:
                return null;
            case WebSocketMessageType.Text:
                return Encoding.UTF8.GetString(ms.ToArray());
            case WebSocketMessageType.Binary:
                await _socket.CloseOutputAsync(WebSocketCloseStatus.InvalidMessageType, "Unexpected binary data", killSocketCt);
                throw new InvalidDataException("Received unexpected binary data");
            default:
                await _socket.CloseOutputAsync(WebSocketCloseStatus.InvalidMessageType, "Invalid message type", killSocketCt);
                throw new InvalidDataException("Received invalid message type");
        }
    }
    private static BackendMessage? ConvertBackendMessage(string json)
    {
        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, [new MyConverter()])
            ?? throw new InvalidDataException("Malformed message");
        NamedArgs args = new(data!);
        if (!args.TryGetValue("messageType", out MessageType type))
            throw new InvalidDataException("Missing messageType");
        if (ReflectionCache.GetType($"SwarmControl.Models.Game.Messages.{type}Message") is not Type messageType)
            throw new InvalidDataException($"Invalid messageType - no message record defined for {type}");
        if (!typeof(BackendMessage).IsAssignableFrom(messageType))
            throw new InvalidDataException("Received a non-backend message from backend");
        try
        {
            return (BackendMessage) (JsonConvert.DeserializeObject(json, messageType, [new MyConverter()])
                ?? throw new InvalidDataException("Failed to deserialize backend message"));
        }
        catch (Exception ex)
        {
            LOGGER.LogError($"Error deserializing: {ex}");
            return null;
        }
    }
    [MemberNotNullWhen(true, nameof(_socket))]
    private bool CheckOpen() => _socket?.State == WebSocketState.Open;
}
