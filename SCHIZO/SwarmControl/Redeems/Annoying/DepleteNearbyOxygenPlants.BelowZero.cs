using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using UnityEngine;

namespace SCHIZO.SwarmControl.Redeems.Annoying;

[Redeem(
    Name = "redeem_o2plants",
    DisplayName = "Deplete O2 Plants"
)]
internal class DepleteNearbyOxygenPlants : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [];

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        foreach (OxygenPlant plant in GameObject.FindObjectsOfType<OxygenPlant>())
        {
            // multiple redeems don't stack
            plant.start = DayNightCycle.main.timePassed;
            plant.UpdateVisuals();
        }
        return CommonResults.OK();
    }
}
