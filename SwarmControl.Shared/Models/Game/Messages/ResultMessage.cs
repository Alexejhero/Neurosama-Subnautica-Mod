using System.Diagnostics.CodeAnalysis;

namespace SwarmControl.Shared.Models.Game.Messages;

#nullable enable
[method: SetsRequiredMembers]
public sealed record ResultMessage() : GameMessage()
{
    public override MessageType MessageType => MessageType.Result;
    public bool Success { get; set; }
    public string? Message { get; set; }
}
