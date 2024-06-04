using System.Collections.Generic;

namespace Control.Models.Game.Messages;

public sealed record InvokeCommandMessage : BackendMessage
{
    public override MessageType MessageType => MessageType.InvokeCommand;
    public CommandInvocationSource Source { get; set; }
    public bool Announce { get; set; } = true;
    public string Command { get; set; } = string.Empty;
    public Dictionary<string, object> Args { get; set; }
}
