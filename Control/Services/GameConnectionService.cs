using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Security.Claims;
using Control.Helpers;
using Control.Models.Game;
using Microsoft.Extensions.Primitives;
using SCHIZO.RemoteControl.Constants;

namespace Control.Services;

public class GameConnectionService(
    IHttpContextAccessor ctxAccess,
    GameConnectionManager manager
    )
{
    private HostAuthInfo? _hostAuthInfo;
    private HttpContext? HttpContext => ctxAccess.HttpContext;

    public Task<string?> RequestTokenAsync(string username, CancellationToken ct = default)
    {
        ClaimsPrincipal? user = HttpContext?.User;
        if (user is null)
            return Task.FromResult<string?>(null);
        return manager.RequestTokenAsync(user, username, ct);
    }

    public async Task<bool> VerifyHostConnectRequest()
    {
        if (HttpContext is null) return false;

        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await HttpContext.Response.WriteAsync("Only WebSocket requests are accepted");
            return false;
        }
        if (!TryGetHeaderSingle(ConnectionConstants.TwitchUsernameHeader, out string? passedUsername)
            || !TryGetHeaderSingle(ConnectionConstants.TokenHeader, out string? passedToken))
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return false;
        }
        if (!HttpContext.TryGetUsername(out string? actualUsername)
            || actualUsername != passedUsername)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return false;
        }
        _hostAuthInfo = new(passedUsername, passedToken);
        if (!manager.VerifyHostAuthInfo(_hostAuthInfo.Value))
            return false;

        return true;
    }

    public async Task HostConnected(WebSocket socket)
    {
        await manager.HostConnected(socket, _hostAuthInfo!.Value.TwitchUsername!);
    }

    private bool TryGetHeaderSingle(string header, [NotNullWhen(true)] out string? value)
    {
        value = default;
        if (HttpContext?.Request.Headers.TryGetValue(header, out StringValues values) != true
            || values is not [{ Length: > 0 } value_])
        {
            return false;
        }
        value = value_;
        return true;
    }
}
