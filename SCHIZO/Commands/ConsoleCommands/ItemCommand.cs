using System.Collections.Generic;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;

namespace SCHIZO.Commands.ConsoleCommands;
[Command(
    Name = "item",
    DisplayName = "Give",
    Description = "Spawns an entity on top of you and puts it in your inventory if it's pickupable"
)]
internal class ItemCommand() : ConsoleWrapperCommand("item")
{
    public override IReadOnlyList<Parameter> Parameters => [
        // this one is typed as string so the concommand can return autocompletes
        new Parameter("techType", typeof(string)),
        new NumericParameter("amount", true),
    ];
}
