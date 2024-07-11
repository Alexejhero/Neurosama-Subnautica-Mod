using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using SCHIZO.Helpers;
using SCHIZO.SwarmControl.Redeems.Enums;

namespace SCHIZO.SwarmControl.Redeems.Audio;
#nullable enable
[Redeem(
    Name = "redeem_noise",
    DisplayName = "Play Sound",
    Announce = false,
    Export = false
)]
internal class PlayNoise : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [
        new Parameter(new("noise", "Sound"), typeof(NormalNoise))
    ];

    protected override object? ExecuteCore(CommandExecutionContext ctx)
    {
        if (!Player.main) return CommonResults.Error("Requires a loaded game.");

        if (!ctx.Input.GetNamedArguments().TryGetValue("noise", out NormalNoise noise)
            || !NoiseRegistry.Normal.TryGetValue(noise, out string path))
        {
            return CommonResults.Error("Noise not found");
        }
        FMODHelpers.PlayOneShotAttached(path, Player.main.gameObject);
        return CommonResults.OK();
    }
}
