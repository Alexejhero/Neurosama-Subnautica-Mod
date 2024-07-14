namespace SCHIZO.SwarmControl.Models.Game.Messages;
internal record PingMessage : GameMessage
{
    public override MessageType MessageType => MessageType.Ping;
}
