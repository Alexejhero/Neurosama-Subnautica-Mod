namespace SCHIZO.SwarmControl.Models.Game.Messages;

#nullable enable
internal record LogMessage : GameMessage
{
    public override MessageType MessageType => MessageType.Log;
    public bool Important { get; set; }
    public string Message { get; set; } = "";
}
