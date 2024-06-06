using System;
using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.ConsoleCommands;

namespace SCHIZO.SwarmControl.Redeems.Spawns;
#nullable enable
internal class SpawnFiltered<T> : ProxyCommand<SpawnCommand>
    where T : struct, Enum
{
    protected virtual string SpawnThingName => "Creature";
    protected virtual string TechTypeParamName => SpawnThingName.ToLowerInvariant();

    public override IReadOnlyList<Parameter> Parameters { get; }

    public SpawnFiltered() : base("spawn")
    {
        Parameters = [
            new(new(TechTypeParamName, SpawnThingName, $"The type of {TechTypeParamName} to spawn."),
                typeof(T)),
            new(new("behind", "Spawn Behind", "If checked, spawn the creature behind the player"),
                typeof(bool), defaultValue: false),
        ];
    }

    protected override Dictionary<string, object?>? GetTargetArgs(Dictionary<string, object?>? proxyArgs)
    {
        if (proxyArgs is null)
            return null;
        Dictionary<string, object?> targetArgs = [];
        if (proxyArgs.TryGetValue("creature", out object? proxyThingTypeBoxed)
            && proxyThingTypeBoxed is T proxyThingType)
        {
            targetArgs["techType"] = Enum.Parse(typeof(TechType), proxyThingType.ToString());
        }
        if (proxyArgs.TryGetValue("behind", out object? behindBoxed)
            && behindBoxed is bool behind)
        {
            targetArgs["distance"] = behind ? -5 : 5;
        }

        return targetArgs;
    }
}
