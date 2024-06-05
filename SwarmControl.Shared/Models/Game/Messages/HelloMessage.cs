using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SwarmControl.Shared.Models.Game;
using SwarmControl.Shared.Models.Game.Messages;

namespace SwarmControl.Shared.Models.Game.Messages;

[method: SetsRequiredMembers]
public sealed record HelloMessage() : GameMessage()
{
    public override MessageType MessageType => MessageType.Hello;
    public List<EnumDefinitionModel> Enums { get; set; }
    public Dictionary<string, List<CommandModel>> Commands { get; set; }
}
