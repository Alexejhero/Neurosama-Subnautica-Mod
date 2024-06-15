using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using SCHIZO.Helpers;
using SCHIZO.SwarmControl.Redeems.Enums;

namespace SCHIZO.SwarmControl.Redeems.Audio;

#nullable enable
[Redeem(
    Name = "redeem_trollnoise",
    DisplayName = "Play Troll Sound",
    Description = "IMPORTANT: Leviathan sound will not play if not underwater",
    Announce = false
)]
internal class PlayTrollNoise : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [
        new(new("noise", "Sound"), typeof(TrollNoise))
    ];

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        if (!Player.main) return CommonResults.Error("Requires a loaded game.");

        if (!ctx.Input.GetNamedArguments().TryGetValue("noise", out TrollNoise noise)
            || !NoiseRegistry.Troll.TryGetValue(noise, out string path))
        {
            return CommonResults.Error("Noise not found");
        }
        FMODHelpers.PlayOneShotAttached(path, Player.main.gameObject);
        return CommonResults.OK();
    }
}
