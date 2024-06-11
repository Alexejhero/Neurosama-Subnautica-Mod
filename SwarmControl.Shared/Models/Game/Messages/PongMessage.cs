namespace SwarmControl.Shared.Models.Game.Messages;

internal record PongMessage : BackendMessage
{
    public override MessageType MessageType => MessageType.Pong;
}
