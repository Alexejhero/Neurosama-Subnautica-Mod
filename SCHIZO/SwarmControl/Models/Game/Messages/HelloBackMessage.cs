namespace SwarmControl.Models.Game.Messages;

public sealed record HelloBackMessage : BackendMessage
{
    public override MessageType MessageType => MessageType.HelloBack;
    public bool Allowed { get; init; }
}
