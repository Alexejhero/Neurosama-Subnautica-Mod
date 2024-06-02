using Newtonsoft.Json;

namespace Control.Models.Game.Messages;

public abstract record GameSocketMessage
{
    [JsonIgnore] // discriminator
    public abstract MessageType MessageType { get; }

    public Guid CorrelationId { get; set; }
    public ulong Timestamp { get; set; }
}
/// <summary>
/// Message sent from the game to the backend.
/// </summary>
public abstract record GameMessage : GameSocketMessage;
/// <summary>
/// Message sent from the backend to the game.
/// </summary>
public abstract record BackendMessage : GameSocketMessage
{
    public TwitchUser? User { get; set; }
}
