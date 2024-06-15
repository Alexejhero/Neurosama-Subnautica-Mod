using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using UnityEngine;

namespace SCHIZO.SwarmControl.Redeems.Annoying;

#nullable enable
[Redeem(
    Name = "redeem_detach",
    DisplayName = "Bad Glue",
    Description = "Whoever designed the Sea Truck module connector should be fired..."
)]
internal class DetachBackSeatruckModule : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [];

    protected override object? ExecuteCore(CommandExecutionContext ctx)
    {
        if (!Player.main) return CommonResults.Error("Requires a loaded game.");

        SeaTruckUpgrades cabin = GameObject.FindObjectOfType<SeaTruckUpgrades>();
        if (!cabin) return CommonResults.Error("Sea Truck not found.");

        SeaTruckSegment cabinSegment = cabin.GetComponent<SeaTruckSegment>();
        SeaTruckSegment rearSegment = cabinSegment;
        while (rearSegment.rearConnection && rearSegment.rearConnection.connection)
            rearSegment = rearSegment.rearConnection.connection.truckSegment;

        if (rearSegment == cabinSegment)
            return CommonResults.Error("Sea Truck not connected to any modules.");

        rearSegment.Detach();

        return CommonResults.OK();
    }
}
