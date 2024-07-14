using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SCHIZO.SwarmControl.Models.Game.Messages;

#nullable enable
[JsonObject(MemberSerialization = MemberSerialization.OptOut, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public abstract record SocketMessage
{
    [JsonRequired]
    public abstract MessageType MessageType { get; }

    public Guid Guid { get; init; } = Guid.NewGuid();
    public long Timestamp { get; } = Now();

    private static long Now() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
/// <summary>
/// Message sent from the game to the backend.
/// </summary>
public abstract record GameMessage : SocketMessage;
/// <summary>
/// Message sent from the backend to the game.
/// </summary>
public abstract record BackendMessage : SocketMessage
{
    public TwitchUser? User { get; set; }

    public string GetUsername() => User?.Login ?? "(Someone)";
    public string GetDisplayName() => User?.DisplayName ?? "(Someone)";
}
