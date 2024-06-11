using System.Diagnostics.CodeAnalysis;

namespace SwarmControl.Models.Game.Messages;

[method: SetsRequiredMembers]
public sealed record HelloBackMessage() : BackendMessage()
{
    public override MessageType MessageType => MessageType.HelloBack;
    public bool Allowed { get; init; }
}
