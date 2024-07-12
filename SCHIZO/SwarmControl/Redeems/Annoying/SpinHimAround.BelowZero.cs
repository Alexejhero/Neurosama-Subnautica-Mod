using System.Collections;
using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using UnityEngine;
using UWE;

namespace SCHIZO.SwarmControl.Redeems.Annoying;
[Redeem(
    Name = "redeem_spin",
    DisplayName = "MODS",
    Description = "Spin him around and make him dizzy"
)]
internal class SpinHimAround : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [];

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        if (!Player.main) return CommonResults.Deny("Requires a loaded game.");

        Player.main.lilyPaddlerHypnosis.StartHypnosis(DayNightCycle.main.timePassed);
        CoroutineHost.StartCoroutine(Coro());

        return CommonResults.OK();
    }

    private IEnumerator Coro()
    {
        yield return new WaitForSeconds(5);
        Player.main.lilyPaddlerHypnosis.StopHypnosis();
    }
}
