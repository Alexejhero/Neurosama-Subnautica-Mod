using System.Diagnostics.CodeAnalysis;

namespace SwarmControl.Shared.Models.Game.Messages;

[method: SetsRequiredMembers]
public sealed record HelloMessage() : GameMessage()
{
    public override MessageType MessageType => MessageType.Hello;
    public string Version { get; init; }
}
