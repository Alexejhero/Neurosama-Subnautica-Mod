namespace SwarmControl.Models.Game.Messages;

public sealed record CommandResultMessage : GameMessage
{
    public override MessageType MessageType => MessageType.CommandResult;
}
