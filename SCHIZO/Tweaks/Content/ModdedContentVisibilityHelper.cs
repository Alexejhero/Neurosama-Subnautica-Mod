using System;
using Nautilus.Commands;
using SCHIZO.ConsoleCommands;
using UnityEngine;

namespace SCHIZO.Tweaks;

[RegisterConsoleCommands]
partial class ModdedContentVisibilityHelper
{
    private float _blinkTimer;
    internal static bool AlertsEnabled { get; private set; }
    private static Action OnAlertsEnabledChanged;

    private void Start()
    {
        OnAlertsEnabledChanged += Changed;
        Changed();
    }

    private void OnDestroy()
    {
        OnAlertsEnabledChanged -= Changed;
    }

    private void OnEnable() => Changed();
    private void OnDisable() => Changed();

    private void Changed()
    {
        _blinkTimer = blinkOnDuration;
        target.gameObject.SetActive(enabled && AlertsEnabled);
    }
    private void LateUpdate()
    {
        if (!AlertsEnabled) return;

        Vector3 pos = Camera.main.transform.position;
        //pos.y = target.pos;
        target.LookAt(pos);

        _blinkTimer -= Time.deltaTime;
        //           <------- timer --------
        // -blinkOffDuration....0....blinkOnDuration
        // ^--------off--------^^--------on--------^
        if (_blinkTimer <= -blinkOffDuration)
            _blinkTimer = blinkOnDuration;
        target.gameObject.SetActive(_blinkTimer >= 0);
    }

    [ConsoleCommand("contentalerts")]
    public static void OnConsoleCommand_contentalerts(bool enable)
    {
        AlertsEnabled = enable;
        OnAlertsEnabledChanged?.Invoke();
    }
}
