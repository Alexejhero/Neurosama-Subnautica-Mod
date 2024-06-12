using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using UnityEngine;
using UWE;

namespace SCHIZO.SwarmControl.Redeems.Annoying;

#nullable enable
[Redeem(
    Name = "redeem_hidecarpings",
    DisplayName = "Hide Vehicle Signals",
    Description = "Hides vehicle and base signals for 30 seconds."
)]
internal class HideVehicleSignal : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [];
    private Coroutine _timer = null!;
    private float _timeLeft;
    private bool _active;
    private static float Duration = 30f;

    private IEnumerator TimerCoro()
    {
        while (true)
        {
            yield return null;
            if (!_active) continue;

            _timeLeft -= Time.deltaTime;
            if (_timeLeft <= 0)
            {
                _timeLeft = 0;
                Deactivate();
            }
        }
    }

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        _timer ??= CoroutineHost.StartCoroutine(TimerCoro());
        _timeLeft += Duration;
        Activate();
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

    private List<PingInstance> _disabledPings = [];

    private void Activate()
    {
        if (_active) return;
        _active = true;
        PingInstance[] allPings = GameObject.FindObjectsOfType<PingInstance>();
        foreach (PingInstance p in allPings.Where(p => _vehiclePingTypes.Contains(p.pingType)))
        {
            if (!p.enabled) continue;

            p.enabled = false;
            _disabledPings.Add(p);
        }
        LOGGER.LogInfo($"{nameof(HideVehicleSignal)} activated - disabled {_disabledPings.Count} pings");
    }
    private void Deactivate()
    {
        if (!_active) return;
        _active = false;
        foreach (PingInstance p in _disabledPings)
        {
            if (!p) continue;
            p.enabled = true;
        }
        LOGGER.LogInfo($"{nameof(HideVehicleSignal)} deactivated");
        _disabledPings.Clear();
    }
}
