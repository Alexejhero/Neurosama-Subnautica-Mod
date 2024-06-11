using System.Collections.Generic;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Input;

namespace SCHIZO.SwarmControl.Redeems.Spawns;

#nullable enable
[CommandCategory("Spawns")]
[Redeem(
    Name = "redeem_isekai",
    DisplayName = "Isekai",
    Description = "Snap a specific type of entity around the player.",
    Announce = AnnounceType.DefaultAnnounce
)]
internal class Isekai : ProxyCommand<MethodCommand>
{
    public enum IsekaiTechType
    {
        Ermfish,
        Anneel,
        Tutel,
        Ermshark,
        Bladderfish,
        Cryptosuchus,
        Titanium,
        Diamond,
        Ruby,
        Gold,
        Copper,
    }
    public enum Range
    {
        Near,
        Far,
    }
    public Isekai() : base("isekai")
    {
        Parameters = [
            new(new("creature", "Creature", "Creature to isekai."), typeof(IsekaiTechType)),
            new(new("range", "Range", null), typeof(Range)),
        ];
    }

    public override IReadOnlyList<Parameter> Parameters { get; }

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
        if (args.TryGetValue("range", out Range range))
        {
            targetArgs["radius"] = range switch
            {
                Range.Near => 20,
                Range.Far => 50,
                _ => 0
            };
        }

        return targetArgs;
    }
}
