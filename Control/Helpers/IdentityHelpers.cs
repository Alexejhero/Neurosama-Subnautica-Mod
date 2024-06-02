using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Control.Helpers;

internal static class IdentityHelpers
{
    public static bool TryGetUsername(this HttpContext ctx, [NotNullWhen(true)] out string? username)
        => TryGetUsername(ctx.User, out username);

    public static bool TryGetUsername(this ClaimsPrincipal user, [NotNullWhen(true)] out string? username)
    {
        username = default;
        if (user is { Identity: { IsAuthenticated: true, Name: { Length: >0 } username_ } })
            username = username_;
        return username is { };
    }
}
