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
using SwarmControl.Shared.Models.Game.Messages;

namespace SCHIZO.SwarmControl;

#nullable enable
internal partial class ControlWebSocket
{
    private readonly ConcurrentQueue<GameMessage> _sendQueue = [];

    private async Task ReceiveThread()
    {
        while (IsConnected)
        {
            string? msg = await ReceiveStringAsync();

            if (msg is null) // close
            {
                LOGGER.LogDebug("Received nothing, closing");
                await _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, default);
                OnClose?.Invoke(_socket.CloseStatus!.Value, _socket.CloseStatusDescription);
                return;
            }

            BackendMessage? message = null;
            try
            {
                message = ConvertBackendMessage(msg);
            }
            catch (InvalidDataException e)
            {
                LOGGER.LogError($"Websocket recv message parse error: {e.Message}");
                OnError?.Invoke(e);
            }
            if (message is { })
                OnMessage?.Invoke(message);
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
                    await SendStringInternal(JsonConvert.SerializeObject(msg, Formatting.None));
                    LOGGER.LogInfo($"Sent {msg.MessageType} ({msg.CorrelationId})");
                }
                catch (Exception e)
                {
                    LOGGER.LogError($"Error sending {msg.GetType().Name} ({msg.CorrelationId})");
                    LOGGER.LogError(e);
                    OnError?.Invoke(e);
                }
            }
            else
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200));
            }
        }
    }

    private async Task SendStringInternal(string message, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentNullException(nameof(message));

        if (!CheckOpen())
            throw new InvalidOperationException("Socket not connected");

        LOGGER.LogDebug($"SEND:\n{message}");
        await _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, ct);
    }

    public void SendMessage(GameMessage message)
    {
        _sendQueue.Enqueue(message);
        LOGGER.LogDebug($"Enqueued {message.MessageType} ({message.CorrelationId})");
    }

    private async Task<string?> ReceiveStringAsync(CancellationToken ct = default)
    {
        if (!CheckOpen())
            return null;

        using MemoryStream ms = new();
        using RentedArray<byte> rent = new(1024, true);

        ArraySegment<byte> buffer = new(rent.Array);
        WebSocketReceiveResult? result = null;
        int totalBytes = 0;
        while (result is not { EndOfMessage: true })
        {
            result = await _socket.ReceiveAsync(buffer, ct);
            ms.Write(buffer.Array, 0, result.Count);
            totalBytes += result.Count;
        }
        LOGGER.LogDebug($"Received {result.MessageType} ({totalBytes} bytes)");
        switch (result.MessageType)
        {
            case WebSocketMessageType.Close:
                return null;
            case WebSocketMessageType.Text:
                return Encoding.UTF8.GetString(ms.ToArray());
            case WebSocketMessageType.Binary:
                await _socket.CloseOutputAsync(WebSocketCloseStatus.InvalidMessageType, "Unexpected binary data", ct);
                throw new InvalidDataException("Received unexpected binary data");
            default:
                await _socket.CloseOutputAsync(WebSocketCloseStatus.InvalidMessageType, "Invalid message type", ct);
                throw new InvalidDataException("Received invalid message type");
        }
    }
    private static BackendMessage ConvertBackendMessage(string json)
    {
        Dictionary<string, object?> data = JsonConvert.DeserializeObject<Dictionary<string, object?>>(json)
            ?? throw new InvalidDataException("Malformed message");
        string? messageTypeName = data.GetOrDefault("messageType") as string;
        if (string.IsNullOrEmpty(messageTypeName))
            throw new InvalidDataException("Missing messageType");
        if (!Enum.TryParse(messageTypeName, out MessageType type) || ReflectionCache.GetType($"SwarmControl.Shared.Models.Game.Messages.{type}Message") is not Type messageType)
            throw new InvalidDataException("Invalid messageType");
        if (!typeof(BackendMessage).IsAssignableFrom(messageType))
            throw new InvalidDataException("Received a non-backend message from backend");
        return (BackendMessage) (JsonConvert.DeserializeObject(json, messageType)
            ?? throw new InvalidDataException("Failed to deserialize backend message"));
    }
    [MemberNotNullWhen(true, nameof(_socket))]
    private bool CheckOpen() => _socket?.State == WebSocketState.Open;
}
