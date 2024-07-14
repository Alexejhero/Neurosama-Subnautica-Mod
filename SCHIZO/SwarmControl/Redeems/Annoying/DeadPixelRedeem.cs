using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Resources;
using UnityEngine;

namespace SCHIZO.SwarmControl.Redeems.Annoying;

#nullable enable
[Redeem(
    Name = "redeem_deadpixel",
    DisplayName = "Dead Pixel",
    Description = "The whole screen is your canvas... Lasts 5 minutes"
)]
internal class DeadPixelRedeem : Command, IParameters
{
    public static float Duration = 300f;
    public IReadOnlyList<Parameter> Parameters => [];

    protected override object? ExecuteCore(CommandExecutionContext ctx)
    {
        Vector2 coords = new(Random.Range(0, 1920), Random.Range(0, 1080));
        GameObject instance = GameObject.Instantiate(Assets.Mod_SwarmControl_DeadPixel, uGUI.main.screenCanvas.transform, false);
        instance.EnsureComponent<DeadPixel>().duration = Duration;
        instance.transform.localPosition = new Vector2(coords.x-960, coords.y-540);
        return $"Your pixel is at {coords}";
    }
}
