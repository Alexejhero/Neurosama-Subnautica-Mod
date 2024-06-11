namespace SwarmControl.Shared.Models.Game.Messages;
internal record PingMessage : GameMessage
{
    public override MessageType MessageType => MessageType.Ping;
}
