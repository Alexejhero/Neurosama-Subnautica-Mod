namespace Control.Models.Game.Messages;

public sealed record HelloMessage : GameMessage
{
    public override MessageType MessageType => MessageType.Hello;
}
