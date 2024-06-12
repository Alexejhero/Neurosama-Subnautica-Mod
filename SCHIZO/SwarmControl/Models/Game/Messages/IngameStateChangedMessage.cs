namespace SwarmControl.Models.Game.Messages;

internal record IngameStateChangedMessage : GameMessage
{
    public override MessageType MessageType => MessageType.IngameStateChanged;
    public bool Ingame { get; set; }
}
