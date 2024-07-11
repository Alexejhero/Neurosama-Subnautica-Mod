namespace SwarmControl.Models.Game.Messages;

#nullable enable
/// <summary>
/// Twitch user info.
/// </summary>
/// <param name="id">Channel ID.</param>
/// <param name="login">Username/channel name.</param>
/// <param name="displayName">The name you see in chat.</param>
public class TwitchUser(string id, string? login = null, string? displayName = null)
{
    /// <summary>Twitch's internal channel ID that corresponds to this user.</summary>
    public string Id { get; } = id;
    /// <summary>Username/channel name.</summary>
    public string Login { get; } = login ?? id;
    /// <summary>The name displayed in chat.</summary>
    public string DisplayName { get; } = displayName ?? login ?? id;
}
