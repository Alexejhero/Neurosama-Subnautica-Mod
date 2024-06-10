using System.Collections.Generic;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;

namespace SCHIZO.SwarmControl.Redeems.Neuro;

#nullable enable
[CommandCategory("Neuro")]
[Command(Name = "redeem_rerollname",
    DisplayName = "Change Vedal's Name",
    Description = "Reroll Vedal's name in the Neuro integration."
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
        "Visage"
    ];

    public override IReadOnlyList<Parameter> Parameters => [];

    protected override Dictionary<string, object?>? GetTargetArgs(Dictionary<string, object?>? proxyArgs)
    {
        Dictionary<string, object?> args = proxyArgs ?? [];
        args["name"] = Names[UnityEngine.Random.Range(0, Names.Length-1)];
        return args;
    }
}
