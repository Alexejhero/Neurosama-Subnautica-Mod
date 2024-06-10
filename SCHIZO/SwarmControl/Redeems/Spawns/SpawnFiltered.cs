using System;
using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.ConsoleCommands;
using SCHIZO.Commands.Input;

namespace SCHIZO.SwarmControl.Redeems.Spawns;

#nullable enable
internal class SpawnFiltered<T> : ProxyCommand<SpawnCommand>
    where T : struct, Enum
{
    protected virtual string SpawnThingName => "Creature";
    protected virtual string TechTypeParamName => SpawnThingName.ToLowerInvariant();
    protected virtual float SpawnDistance => 5;

    public override IReadOnlyList<Parameter> Parameters { get; }

    public SpawnFiltered() : base("spawn")
    {
        Parameters = [
            new(new(TechTypeParamName, SpawnThingName, $"The type of {TechTypeParamName} to spawn."),
                typeof(T)),
            new(new("behind", "Spawn Behind", "Spawn the creature behind the player"),
                typeof(bool), defaultValue: false),
        ];
    }

    protected override Dictionary<string, object?>? GetTargetArgs(Dictionary<string, object?>? proxyArgs)
    {
        if (proxyArgs is null)
            return null;
        NamedArgs args = new(proxyArgs);
        Dictionary<string, object?> targetArgs = [];
        if (args.TryGetValue(TechTypeParamName, out T proxyThingType))
        {
            targetArgs["techType"] = proxyThingType.ToString();
        }
        else
        {
            // console command will validate and return this in the error result
            // the prefix is to avoid it actually parsing into a valid tech type
            targetArgs["techType"] = $"INVALID_{args.GetOrDefault(TechTypeParamName, "null")}";
        }
        bool behind = args.GetOrDefault("behind", false);
        targetArgs["distance"] = behind ? -SpawnDistance : SpawnDistance;

        return targetArgs;
    }
}
