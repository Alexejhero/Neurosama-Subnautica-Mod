using System.Collections.Generic;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.ConsoleCommands;
using SwarmControl.Shared.Models.Game;

namespace SCHIZO.SwarmControl.Redeems.Neuro;

[CommandCategory("Neuro")]
[Redeem(Name = COMMAND,
    DisplayName = "Set Vedal's Name",
    Description = "Set Vedal's name in the Neuro integration.",
    Announce = AnnounceType.DefaultAnnounce
)]
internal class SetPlayerName() : ConsoleWrapperCommand("immersion set player")
{
    public const string COMMAND = "redeem_setname";

    public override IReadOnlyList<Parameter> Parameters => [
        new TextParameter(new NamedModel("name"))
    ];
}
