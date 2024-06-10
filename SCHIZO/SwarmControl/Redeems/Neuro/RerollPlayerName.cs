using System.Collections.Generic;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;

namespace SCHIZO.SwarmControl.Redeems.Neuro;

#nullable enable
[CommandCategory("Neuro")]
[Redeem(
    Name = "redeem_rerollname",
    DisplayName = "Reroll Vedal's Name",
    Description = "Reroll Vedal's name in the Neuro integration.",
    Announce = AnnounceType.DefaultAnnounce
)]
internal class RerollPlayerName() : ProxyCommand<SetPlayerName>(SetPlayerName.COMMAND)
{
    public static readonly string[] Names = [
        "Vedal",
        "Vega",
        "Bucket",
        "Vedalopalypse",
        "Vedalitaire",
        "Vadel",
        "Vercel",
        "Vorpal",
        "Vedalite",
        "Vita",
        "Voronoi",
        "Vernal",
        "Tutel",
        "Visage",
        "Dalveed",
        "Harrison Temple",
        "Hypervedal",
        "Mosquito987",
        "Coldfish",
        "Ladev",
        "Vedeal",
        "Veed",
        "Ved",
        "Waddle",
        "Weedle",
        "Wordle",
        "Veddie",
        "Vedalyn",
        "Vortex",
        "Lemon"
    ];

    public override IReadOnlyList<Parameter> Parameters => [];

    protected override Dictionary<string, object?>? GetTargetArgs(Dictionary<string, object?>? proxyArgs)
    {
        Dictionary<string, object?> args = proxyArgs ?? [];
        args["name"] = Names[UnityEngine.Random.Range(0, Names.Length-1)];
        return args;
    }
}
