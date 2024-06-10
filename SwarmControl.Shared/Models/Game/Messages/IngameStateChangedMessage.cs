namespace SwarmControl.Shared.Models.Game.Messages;

internal record IngameStateChangedMessage : GameMessage
{
    public override MessageType MessageType => MessageType.IngameStateChanged;
    public bool Ingame { get; set; }
    public bool Paused { get; set; }
    /// <summary>
    /// Forbid spawning while inside a base or seatruck (prawn suit is fine)
    /// </summary>
    public bool Indoors { get; set; }
    public bool OnLand { get; set; }
}
