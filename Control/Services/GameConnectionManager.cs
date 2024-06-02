using System.Net.WebSockets;
using System.Security.Claims;
using System.Security.Cryptography;
using Control.Helpers;
using Control.Models.Game;

namespace Control.Services;

public class GameConnectionManager(ILogger<GameConnectionManager> logger) : IHostedService
{
    // sophisticated database technology
    private static readonly Dictionary<string, WebSocket> _openHosts = [];
    private static readonly Dictionary<string, Dictionary<string, WebSocket>> _openClients = [];
    private static readonly Dictionary<string, int> _tokens = [];

    public async Task<string?> RequestTokenAsync(ClaimsPrincipal user, string username, CancellationToken ct = default)
    {
        if (!user.TryGetUsername(out string? actualUser) || actualUser != username)
        {
            logger.LogTrace("Unauth user {Username} ({ActualUser}) requesting token", username, actualUser);
            return null;
        }
        if (_tokens.TryGetValue(username, out _))
        {
            logger.LogTrace("User {Username} requesting token, removing duplicate connection", username);
            _tokens.Remove(username);
            await DisconnectHostAsync(username, "Duplicate connection", ct);
        }

        logger.LogTrace("Generating token for {Username}", username);
        string token = GenerateToken(64);
        logger.LogTrace("Generated {Token} ({Hash})", token, token.GetHashCode());
        _tokens[username] = token.GetHashCode();
        return token;
    }

    private static async Task DisconnectHostAsync(string username, string? reason = null, CancellationToken ct = default)
    {
        if (_openHosts.TryGetValue(username, out WebSocket? socket))
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, reason, ct);
            _openHosts.Remove(username);
        }
    }

    public async Task HostConnected(WebSocket socket, string username, CancellationToken ct = default)
    {
        await DisconnectHostAsync(username, "Duplicate connection", ct);

        _openHosts[username] = socket;
    }

    public async Task ConnectControlAsync(HttpContext ctx, string hostUsername)
    {
        if (ctx.TryGetUsername(out string? clientUsername))
        {
        }
    }

    public Task StartAsync(CancellationToken _)
    {
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken _)
    {
        await Task.WhenAll(_openClients.Values.SelectMany(connections => connections.Values).Select(CloseConnection));
        _openClients.Clear();

        await Task.WhenAll(_openHosts.Values.Select(CloseConnection));
        _openHosts.Clear();

        static Task CloseConnection(WebSocket conn)
        {
            if (conn.State == WebSocketState.Closed)
                return Task.CompletedTask;
            // CloseOutputAsync doesn't wait for the client to reply with its own close message (whereas CloseAsync does)
            return conn.CloseOutputAsync(WebSocketCloseStatus.EndpointUnavailable, "Service stopping", default);
        }
    }

    public bool VerifyHostAuthInfo(HostAuthInfo hostAuthInfo)
    {
        return _tokens.TryGetValue(hostAuthInfo.TwitchUsername, out int tokenHashCode)
            && tokenHashCode == hostAuthInfo.Token.GetHashCode();
    }

    private static string GenerateToken(int bytes)
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        Span<byte> buf = stackalloc byte[bytes];
        rng.GetBytes(buf);
        return Convert.ToBase64String(buf);
    }
}
