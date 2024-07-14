namespace SCHIZO.SwarmControl.Models.Game.Messages;

public sealed record ConsoleInputMessage : BackendMessage
{
    public override MessageType MessageType => MessageType.ConsoleInput;
    public string Input { get; set; } = string.Empty;
}
