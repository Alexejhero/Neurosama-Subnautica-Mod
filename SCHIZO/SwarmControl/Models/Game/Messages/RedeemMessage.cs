using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SwarmControl.Models.Game.Messages;

#nullable enable
[method: SetsRequiredMembers]
public sealed record RedeemMessage() : BackendMessage()
{
    public override MessageType MessageType => MessageType.Redeem;

    public CommandInvocationSource Source { get; set; }
    public bool Announce { get; set; } = true;
    public string Title { get; set; } = string.Empty;
    public string Command { get; set; } = string.Empty;
    public Dictionary<string, object?>? Args { get; set; }
}
