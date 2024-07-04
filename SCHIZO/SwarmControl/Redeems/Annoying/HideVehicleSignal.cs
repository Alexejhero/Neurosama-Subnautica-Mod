using System.Collections.Generic;
using System.Linq;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using UnityEngine;

namespace SCHIZO.SwarmControl.Redeems.Annoying;

#nullable enable
[Redeem(
    Name = "redeem_hidecarpings",
    DisplayName = "Hide Vehicle Signals",
    Description = "Hides vehicle and base signals for 1 minute."
)]
internal class HideVehicleSignal : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [];
    private readonly EggTimer _timer;
    internal static float Duration = 60f;

    public HideVehicleSignal()
    {
        _timer = new(Activate, Deactivate);
    }

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        _timer.AddTime(Duration);
        _timer.Start();
        return CommonResults.OK();
    }

    private static readonly HashSet<PingType> _vehiclePingTypes = [
        PingType.Exosuit,
        PingType.Seamoth,
        PingType.Cyclops,
        PingType.Base,
#if BELOWZERO
        PingType.SeatruckMainCabin,
        PingType.SeatruckMainCabinWithModules,
        PingType.SeatruckModule,
        PingType.ControlRoom,
        PingType.Hoverbike,
#endif
    ];

    private readonly List<PingInstance> _disabledPings = [];

    private void Activate()
    {
        PingInstance[] allPings = GameObject.FindObjectsOfType<PingInstance>();
        foreach (PingInstance p in allPings.Where(p => _vehiclePingTypes.Contains(p.pingType)))
        {
            if (!p.enabled) continue;

            p.OnDisable(); // we don't want to store them as disabled in the save file
            _disabledPings.Add(p);
        }
        LOGGER.LogInfo($"{nameof(HideVehicleSignal)} activated - disabled {_disabledPings.Count} pings");
    }
    private void Deactivate()
    {
        foreach (PingInstance p in _disabledPings)
        {
            if (!p) continue;
            p.OnEnable();
        }
        LOGGER.LogInfo($"{nameof(HideVehicleSignal)} deactivated");
        _disabledPings.Clear();
    }
}
