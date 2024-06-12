namespace SwarmControl.Models.Game.Messages;

internal record IngameStateChangedMessage : GameMessage
{
    public override MessageType MessageType => MessageType.IngameStateChanged;
    public bool Ingame { get; set; }
    /// <summary>
    /// Spawning is forbidden while on land or inside a base/seatruck (prawn suit is fine)
    /// </summary>
    public bool CanSpawn { get; set; }
}
