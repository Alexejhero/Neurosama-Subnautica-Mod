using System.Security.Claims;
using AspNet.Security.OAuth.Twitch;

namespace Control.Services;

public class UserService(IHttpContextAccessor http)
{
    public string GetNameForDisplay()
        => GetDisplayName() ?? GetUserName() ?? "Anonymous";

    private string? GetUserName() => http.HttpContext?.User.Identity?.Name;
    public string? GetDisplayName() => http.HttpContext?.User.FindFirstValue(TwitchAuthenticationConstants.Claims.DisplayName);
}
