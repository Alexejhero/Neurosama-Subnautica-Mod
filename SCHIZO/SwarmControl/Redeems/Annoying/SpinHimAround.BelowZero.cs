using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;

namespace SCHIZO.SwarmControl.Redeems.Annoying;
[Redeem(
    Name = "redeem_spin",
    DisplayName = "Spin him around and make him dizzy"
)]
internal class SpinHimAround : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [];

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        if (!Player.main) return CommonResults.Error("Requires a loaded game.");

        Player.main.lilyPaddlerHypnosis.StartHypnosis(DayNightCycle.main.timePassed);

        return CommonResults.OK();
    }
}
