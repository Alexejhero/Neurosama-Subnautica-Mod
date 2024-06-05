using System.Diagnostics.CodeAnalysis;

namespace SwarmControl.Shared.Models.Game.Messages;

[method: SetsRequiredMembers]
public sealed record HelloBackMessage() : BackendMessage()
{
    public override MessageType MessageType => MessageType.HelloBack;
}
