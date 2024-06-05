using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using System.Text.Json;
using Newtonsoft.Json.Serialization;
using SwarmControl.Shared.Models.Game.Messages;

namespace SwarmControl.Shared.Models.Game.Messages;

#nullable enable
[method: SetsRequiredMembers]
public sealed record InvokeCommandMessage() : BackendMessage()
{
    public override MessageType MessageType => MessageType.InvokeCommand;

    public CommandInvocationSource Source { get; set; }
    public bool Announce { get; set; } = true;
    public string Command { get; set; } = string.Empty;
    public Dictionary<string, object>? Args { get; set; }
}