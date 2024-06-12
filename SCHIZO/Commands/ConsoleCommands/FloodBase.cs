using System.Collections.Generic;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;
using SwarmControl.Models.Game;

namespace SCHIZO.Commands.ConsoleCommands;
#nullable enable
[Command(Name = "damagebase",
    DisplayName = "Damage Base",
    Description = "Damages all leakable base cells."
)]
internal class FloodBase() : ConsoleWrapperCommand("damagebase")
{
    public override IReadOnlyList<Parameter> Parameters => [
        new NumericParameter(new NamedModel("damage"), false, 20),
    ];
}
