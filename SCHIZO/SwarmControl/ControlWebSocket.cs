using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using SCHIZO.Helpers;
using SwarmControl.Constants;
using SwarmControl.Shared.Models.Game.Messages;
using SwarmControl.Shared.Models.Game;

namespace SCHIZO.SwarmControl;

#nullable enable
internal partial class ControlWebSocket : IDisposable
{
    public event Action? OnConnected;
    public event Action<BackendMessage>? OnMessage;
    public event Action<Exception>? OnError;
    public event Action<WebSocketCloseStatus, string?>? OnClose;

    public bool IsConnected => _socket.State == WebSocketState.Open;

    private ClientWebSocket _socket = new();
    private bool _isDisposed;

    private ClientWebSocket MakeNewSocket()
    {
        _socket?.Dispose();
        return new()
        {
            Options = { KeepAliveInterval = TimeSpan.FromSeconds(5) }
        };
    }
    public Uri BaseUri => new(SwarmControlManager.Instance.BackendUrl);

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
        if (!BaseUri.Scheme.StartsWith("http")) // no :^)
            return $"""
                Backend url is invalid
                Press Shift+Enter to open the console
                then enter "{SwarmControlManager.COMMAND_URL} <backend url>"
                """;

        // check if the server responds at all
        try
        {
            int timeout = BaseUri.IsLoopback ? 1 : 5;

            using HttpClient client = new() { BaseAddress = BaseUri, Timeout = TimeSpan.FromSeconds(timeout) };
            HttpRequestMessage request = new(HttpMethod.Options, "/");
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
        if (!await ConnectSocket())
            return "Could not establish socket connection, try again";

        return "Connected";
    }

    public async Task<bool> ConnectSocket()
    {
        if (IsConnected) return true;

        if (_socket is not { State: WebSocketState.None })
        {
            // you can't reuse a socket that has already been opened at any point
            LOGGER.LogWarning("Socket was opened, remaking");
            if (_socket?.State is WebSocketState.Open or WebSocketState.CloseSent or WebSocketState.CloseReceived)
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Reestablishing connection", default);
            _socket = MakeNewSocket();
        }

        Uri wsBaseUri = new UriBuilder(BaseUri) { Scheme = BaseUri.Scheme == "https" ? "wss" : "ws" }.Uri;
        Uri hostWebsocketUri = new(wsBaseUri, ConnectionConstants.ServerHostWebsocketEndpoint);

        _socket.Options.SetRequestHeader("Authorization", $"Bearer {SwarmControlManager.Instance.PrivateApiKey}");

        try
        {
            LOGGER.LogInfo("Connecting to websocket");
            await _socket.ConnectAsync(hostWebsocketUri, default);
            if (_socket.State != WebSocketState.Open)
            {
                return false;
            }
        }
        catch (Exception e)
        {
            LOGGER.LogWarning("failed to connect ws");
            if (e is not WebSocketException)
                LOGGER.LogError(e);
            // socket disposes itself on connection error
            _socket = MakeNewSocket();
            return false;
        }
        OnConnected?.Invoke();
#pragma warning disable CS4014 // on purpose, these are background threads
        Task.Run(SendThread);
        Task.Run(ReceiveThread);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        return true;
    }

    public async Task<string?> Disconnect()
    {
        if (!IsConnected) return null;

        LOGGER.LogInfo("Disconnecting websocket");
        try
        {
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, default);
            // don't call OnClose since it's an explicit disconnect request
        }
        catch { } // she'll be right
        _socket = MakeNewSocket();
        return "Disconnected";
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        _socket?.Dispose();
    }
}
