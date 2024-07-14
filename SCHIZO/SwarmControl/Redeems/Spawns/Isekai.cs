using System.Collections.Generic;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Input;

namespace SCHIZO.SwarmControl.Redeems.Spawns;

#nullable enable
[CommandCategory("Spawns")]
[Redeem(
    Name = "redeem_isekai",
    DisplayName = "Remove Entities",
    Description = "Snap a specific type of entity around the player. Does not include inventory/containers."
)]
internal class Isekai() : ProxyCommand<MethodCommand>("isekai")
{
    public enum IsekaiTechType
    {
        Ermfish,
        Anneel,
        Tutel,
        Ermshark,
        Bladderfish,

        Titanium,
        Copper,
        Silver,
        Lead,
        Gold,
        Diamond,
        Ruby,
    }

    public override IReadOnlyList<Parameter> Parameters { get; }
        = [new(new("creature", "Type", "Which entities to remove."), typeof(IsekaiTechType))];

    protected override Dictionary<string, object?>? GetTargetArgs(Dictionary<string, object?>? proxyArgs)
    {
        if (proxyArgs is null)
            return null;
        Dictionary<string, object?> targetArgs = [];
        NamedArgs args = new(proxyArgs);
        if (args.TryGetValue("creature", out IsekaiTechType proxyThingType))
        {
            targetArgs["techType"] = proxyThingType.ToString();
        }
        targetArgs["proportion"] = 1f;
        targetArgs["radius"] = 50f;

        return targetArgs;
    }
}
