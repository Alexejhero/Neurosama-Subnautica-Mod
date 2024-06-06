using System.Collections.Generic;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;

namespace SCHIZO.Commands.ConsoleCommands;

[Command(Name = "spawn",
    DisplayName = "Spawn",
    Description = "Spawn entities of the specified type.")]
internal sealed class SpawnCommand() : ConsoleWrapperCommand("spawn")
{
    private static readonly Parameter[] _parameters = [
        new("techType", typeof(TechType), false, "Tech type", "The tech type to spawn."),
        new("count", typeof(int), true, "Count", "Amount of entities to spawn."),
        new("distance", typeof(float), true, "Distance", "Maximum distance from the player. Can be negative to spawn behind.")
    ];
    public override IReadOnlyList<Parameter> Parameters => [.. _parameters];
}
