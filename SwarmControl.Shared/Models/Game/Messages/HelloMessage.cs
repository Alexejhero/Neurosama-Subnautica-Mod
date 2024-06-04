using System.Collections.Generic;
using SwarmControl.Shared.Models.Game;

namespace SwarmControl.Models.Game.Messages;

public sealed record HelloMessage : GameMessage
{
    public override MessageType MessageType => MessageType.Hello;
    public List<EnumDefinitionModel> Enums { get; set; }
    public List<CommandModel> Commands { get; set; }
}
