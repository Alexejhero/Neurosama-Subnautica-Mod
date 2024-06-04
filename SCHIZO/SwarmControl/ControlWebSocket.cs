using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SwarmControl.Models.Game;
using SwarmControl.Models.Game.Messages;
using Newtonsoft.Json;
using SCHIZO.Helpers;
using SwarmControl.Constants;

namespace SCHIZO.SwarmControl;

#nullable enable
internal class ControlWebSocket : IDisposable
{
    private ClientWebSocket _socket = new();
    private HostAuthInfo? _authInfo;
    private string _nonce = null!;
    private bool _isDisposed;

    public bool IsConnected => _socket.State == WebSocketState.Open;


    private ClientWebSocket MakeNewSocket()
    {
        _socket?.Dispose();
        return new()
        {
            Options = { KeepAliveInterval = TimeSpan.FromSeconds(5) }
        };
    }
    /// <summary>
    /// Kick off the flow for connecting to the backend server.<br/>
    /// This will open a new browser window or tab so the backend can authenticate the user to Twitch.<br/>
    /// When the user is authenticated, the browser will send back the <see cref="HostAuthInfo"/>.<br/>
    /// The game will use it when establishing a web socket connection to the backend.
    /// </summary>
    /// <param name="ct">Token to cancel connecting.</param>
    /// <returns>If an error occurred, a <see cref="string"/> describing the error (to be shown to the user). Otherwise, <see langword="null"/>.</returns>
    public async Task<string?> Connect(CancellationToken ct = default)
    {
        // open browser
        Uri baseUri = new(SwarmControlManager.Instance.BackendUrl);
        if (!baseUri.Scheme.StartsWith("http")) // no :^)
            return """
                Backend url is invalid
                Press Shift+Enter to open the console
                then enter "setswarmcontrolurl <backend url>"
                """;
        if (_socket is not { State: WebSocketState.None })
        {
            LOGGER.LogWarning("Socket was opened, closing");
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Reestablishing connection", default);
            _socket = MakeNewSocket();
        }

        // check if the server responds at all
        try
        {
            int timeout = baseUri.IsLoopback ? 1 : 5;

            using HttpClient client = new() { BaseAddress = baseUri, Timeout = TimeSpan.FromSeconds(timeout) };
            HttpRequestMessage request = new(HttpMethod.Head, "/");
            await client.SendAsync(request, ct);
        }
        catch (Exception e)
        {
            if (ct.IsCancellationRequested)
                return null;
            if (e is HttpRequestException or OperationCanceledException)
                return "No response from server, it may be offline";
            else
                throw;
        }

        using (RentedArray<byte> rent = new(32, true))
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                rng.GetBytes(rent.Array);
            _nonce = Convert.ToBase64String(rent.Array);
        }
        // listen for auth info incoming from browser after twitch auth
        ushort port = NetHelpers.TryGetFreePort()
            ?? (ushort) UnityEngine.Random.Range(20000, 45000); // god will pick a port and it shall be free

        using HttpListener listener = new();
        string uriPrefix = $"http://*:{port}/";
        listener.Prefixes.Add(uriPrefix);
        try
        {
            listener.Start();
            LOGGER.LogInfo($"Listening on {uriPrefix}");
        }
        catch (Exception e)
        {
            LOGGER.LogWarning($"Could not start listener on {uriPrefix}");
            LOGGER.LogError(e);
            return "Could not start listener";
        }

        Uri hostLoginUri = new(baseUri, $"{ConnectionConstants.ServerHostEndpoint}?nonce={Uri.EscapeDataString(_nonce)}&port={port}");
        // top 10 windows funny moments
        Process.Start("explorer", $"\"{hostLoginUri}\"");

        try
        {
            _authInfo = await GetTokenAsync(listener, ct);
        }
        catch (Exception e)
        {
            if (e is OperationCanceledException)
                return null; // user cancelled

            LOGGER.LogWarning("Error while waiting for response");
            LOGGER.LogError(e);
            string msg = "Could not establish connection, try again";
            if (e is InvalidOperationException ioe)
                msg += "\n" + ioe.Message;
            return msg;
        }
        if (_authInfo is not { TwitchUsername.Length: > 0, Token.Length: > 0 })
            return "Invalid response received\nBackend server may be misconfigured";

        Uri wsBaseUri = new UriBuilder(baseUri) { Scheme = baseUri.Scheme == "https" ? "wss" : "ws" }.Uri;
        Uri hostWebsocketUri = new(wsBaseUri, ConnectionConstants.ServerHostWebsocketEndpoint);

        _socket.Options.SetRequestHeader(ConnectionConstants.TwitchUsernameHeader, _authInfo?.TwitchUsername);
        _socket.Options.SetRequestHeader(ConnectionConstants.TokenHeader, Uri.EscapeDataString(_authInfo?.Token));

        try
        {
            LOGGER.LogInfo("Connecting to websocket");
            await _socket.ConnectAsync(hostWebsocketUri, default);
        }
        catch (Exception e)
        {
            LOGGER.LogWarning("surprise connection renegotiation");
            LOGGER.LogError(e);
            // socket disposes itself on connection error
            _socket = MakeNewSocket();
            return "Could not establish socket connection, try again";
        }
        return $"Connected as {_authInfo?.TwitchUsername}";

        async Task<HostAuthInfo?> GetTokenAsync(HttpListener listener, CancellationToken ct = default)
        {
            int nonceHashCode = _nonce.GetHashCode();
            while (!ct.IsCancellationRequested)
            {
                HttpListenerContext? ctx = await GetContextAsyncCancellable(listener, ct);
                if (ctx is null) continue;

                //LOGGER.LogDebug($"{ctx.Request.HttpMethod} {ctx.Request.Url} from {ctx.Request.RemoteEndPoint.Address}");
                ctx.Response.AppendHeader("Access-Control-Allow-Origin", "*");

                if (ctx.Request.HttpMethod != "POST"
                    || ctx.Request.Url.LocalPath != "/reply"
                    || ctx.Request.QueryString["nonce"]?.GetHashCode() != nonceHashCode)
                {
                    LOGGER.LogWarning($"Invalid request from {ctx.Request.RemoteEndPoint.Address}");
                    ctx.Response.StatusCode = 400;
                    ctx.Response.Close();
                    continue;
                }
                string username = ctx.Request.QueryString["username"];
                string token = ctx.Request.QueryString["token"];
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
                {
                    LOGGER.LogWarning("Request without credentials");
                    ctx.Response.StatusCode = 401;
                    ctx.Response.Close();
                    continue;
                }
                LOGGER.LogInfo("Received credentials");
                ctx.Response.StatusCode = 204;
                ctx.Response.Close();
                return new(username, token);
            }

            return null;
        }

        static async Task<HttpListenerContext?> GetContextAsyncCancellable(HttpListener listener, CancellationToken ct = default)
        {
            return await Task.Run(() =>
            {
                Task<HttpListenerContext> bruh = listener.GetContextAsync();
                bruh.Wait(ct);
                if (!ct.IsCancellationRequested) // not cancelled means the task completed
                    return bruh.Result;
                return null;
            });
        }
    }

    public async Task<string?> Disconnect()
    {
        if (!IsConnected) return null;

        LOGGER.LogInfo("Disconnecting websocket");
        try
        {
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, default);
        }
        catch { } // she'll be right
        _socket = MakeNewSocket();
        return "Disconnected";
    }

    public async Task SendStringAsync(string message, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentNullException(nameof(message));

        ValidateOpen();

        await _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, ct);
    }

    public async Task SendJsonAsync(GameMessage toSerialize)
        => await SendStringAsync(JsonConvert.SerializeObject(toSerialize, Formatting.None));

    public async Task<string> ReceiveStringAsync(CancellationToken ct = default)
    {
        ValidateOpen();

        using MemoryStream ms = new();
        using RentedArray<byte> rent = new(1024, true);

        ArraySegment<byte> buffer = new(rent.Array);
        WebSocketReceiveResult? result = null;
        while (result is not { EndOfMessage: true })
        {
            result = await _socket.ReceiveAsync(buffer, ct);
            if (result.MessageType != WebSocketMessageType.Text)
            {
                await _socket.CloseOutputAsync(WebSocketCloseStatus.InvalidMessageType, "Unexpected binary data", ct);
                throw new InvalidDataException("Received unexpected binary data");
            }
            ms.Write(buffer.Array, 0, result.Count);
        }
        return Encoding.UTF8.GetString(ms.ToArray());
    }

    [MemberNotNull(nameof(_socket))]
    private void ValidateOpen()
    {
        if (_socket?.State != WebSocketState.Open)
            throw new InvalidOperationException("Socket not connected");
    }

    public async Task<BackendMessage> ReceiveMessageAsync(CancellationToken ct = default)
    {
        string json = await ReceiveStringAsync(ct);
        Dictionary<string, object?> data = JsonConvert.DeserializeObject<Dictionary<string, object?>>(json)
            ?? throw new InvalidDataException("Malformed message");
        string? messageTypeName = data.GetOrDefault("messageType") as string;
        if (string.IsNullOrEmpty(messageTypeName))
            throw new InvalidDataException("Missing messageType");
        if (!Enum.TryParse(messageTypeName, out MessageType type) || ReflectionCache.GetType($"Control.Models.Game.Messages.{type}Message") is not Type messageType)
            throw new InvalidDataException("Invalid messageType");
        if (!typeof(BackendMessage).IsAssignableFrom(messageType))
            throw new InvalidDataException("Received a non-backend message from backend");
        return (BackendMessage) (JsonConvert.DeserializeObject(json, messageType)
            ?? throw new InvalidDataException("Failed to deserialize backend message"));
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        _socket?.Dispose();
    }
}
