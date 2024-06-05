using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SwarmControl.Shared.Models.Game.Messages;

#nullable enable
[JsonObject(MemberSerialization = MemberSerialization.OptOut, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public abstract record GameSocketMessage
{
    [JsonRequired]
    public abstract MessageType MessageType { get; }

    public Guid CorrelationId { get; init; }
    public long Timestamp { get; }

    public GameSocketMessage()
    {
        Timestamp = Now();
        CorrelationId = Guid.NewGuid();
    }

    private static long Now() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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

    public string GetUsername() => User?.Username ?? "(Anonymous)";
    public string GetDisplayName() => User?.DisplayName ?? "(Anonymous)";
}
