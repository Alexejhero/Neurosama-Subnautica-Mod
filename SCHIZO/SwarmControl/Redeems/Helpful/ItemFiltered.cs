using System;
using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.ConsoleCommands;
using SCHIZO.Commands.Input;

namespace SCHIZO.SwarmControl.Redeems.Helpful;

#nullable enable
internal class ItemFiltered<T> : ProxyCommand<ItemCommand>
    where T : struct, Enum
{
    protected virtual string SpawnThingName => "Item";
    protected virtual string TechTypeParamName => SpawnThingName.ToLowerInvariant();

    public override IReadOnlyList<Parameter> Parameters { get; }

    public ItemFiltered() : base("item")
    {
        Parameters = [
            new(new(TechTypeParamName, SpawnThingName, $"The type of {TechTypeParamName} to give."),
                typeof(T)),
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

        targetArgs["amount"] = 1;

        return targetArgs;
    }
}
