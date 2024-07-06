using System.Collections.Generic;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;
using SwarmControl.Models.Game;

namespace SCHIZO.Commands.ConsoleCommands;

[Command(Name = "spawn",
    DisplayName = "Spawn",
    Description = "Spawn entities of the specified type.")]
internal sealed class SpawnCommand() : ConsoleWrapperCommand("spawn")
{
    private static readonly Parameter[] _parameters = [
        new(new NamedModel("techType", "TechType", "The tech type to spawn."), typeof(TechType)),
        new NumericParameter(new NamedModel("count", "Count", "Amount of entities to spawn."), true, 1),
        new NumericParameter(new NamedModel("distance", "Distance", "Maximum distance from the player. Spawns behind if negative."), false, true),
        new NumericParameter(new NamedModel("lifetime", "Lifetime", "(Internal) The spawned object will be auto-destroyed in this many seconds"), false, float.PositiveInfinity),
    ];
    public override IReadOnlyList<Parameter> Parameters => [.. _parameters];
}
