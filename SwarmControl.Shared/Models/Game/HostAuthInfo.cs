namespace SwarmControl.Shared.Models.Game;

/// <summary>
/// Authentication information for a "host" connection to the backend.
/// </summary>
/// <param name="twitchUsername">Identity of the user that logged into the backend.</param>
/// <param name="token">Opaque token.</param>
public readonly struct HostAuthInfo(string twitchUsername, string token)
{
    public string TwitchUsername { get; } = twitchUsername;
    public string Token { get; } = token;
}
