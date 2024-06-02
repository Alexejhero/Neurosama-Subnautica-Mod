namespace Control.Models.Game.Messages;

public sealed record CommandResultMessage : GameMessage
{
    public override MessageType MessageType => MessageType.CommandResult;
}
