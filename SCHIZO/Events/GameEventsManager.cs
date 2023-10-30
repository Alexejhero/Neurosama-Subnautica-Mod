using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Nautilus.Commands;
using Nautilus.Utility;
using SCHIZO.Attributes.Loading;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Events;

[LoadConsoleCommands]
public partial class GameEventsManager
{
    public static GameEventsManager Instance { get; private set; }
    public static List<GameEvent> Events { get; private set; } = new();

    private const string AUTOEVENTS_PREFS_KEY = "SCHIZO_Events_AutoEvents";
    public static bool HasBeenSet => PlayerPrefs.HasKey(AUTOEVENTS_PREFS_KEY);
    public bool AutoStartEvents
    {
        get => GetAutoStart(autoStartEvents);
        set => SetAutoStart(value);
    }

    public static bool GetAutoStart(bool defaultVal = true) => PlayerPrefsExtra.GetBool(AUTOEVENTS_PREFS_KEY, defaultVal);
    public static void SetAutoStart(bool value) => PlayerPrefsExtra.SetBool(AUTOEVENTS_PREFS_KEY, value);

    private void Awake()
    {
        if (Instance) Destroy(Instance);
        Instance = this;
        gameObject.GetComponents(Events);

        if (!HasBeenSet || overridePlayerPrefs) SetAutoStart(autoStartEvents);

        DevConsole.RegisterConsoleCommand(this, "autoevents", false, true);
    }

    // not an attribute command because those don't support optional parameters
    [UsedImplicitly]
    private void OnConsoleCommand_autoevents(NotificationCenter.Notification n)
    {
        if (n.data is not { Count: > 0})
        {
            MessageHelpers.WriteCommandOutput($"Events are currently {FormatAutoStart(GetAutoStart())}");
            return;
        }

        if (ConsoleHelpers.TryParseBoolean(n.data[0] as string, out bool start))
        {
            SetAutoStart(start);
            MessageHelpers.WriteCommandOutput($"Events are now {FormatAutoStart(GetAutoStart())}");
        }
        else
        {
            MessageHelpers.WriteCommandOutput($"Syntax: autoevents [on|off]");
        }
    }

#if DEBUG
    public void Update()
    {
        if (!Input.GetKeyDown(KeyCode.LeftControl)) return;
        if (!Player.main || !Player.main.guiHand || !Player.main.guiHand.activeTarget) return;

        ErmconAttendee target = Player.main.guiHand.activeTarget.GetComponent<ErmconAttendee>();
        if (!target) return;

        target.CycleDebug();
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string FormatAutoStart(bool value) => value ? "automatic" : "manual";
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string FormatStartEnd(bool start) => start ? "start" : "end";

    [ConsoleCommand("event"), UsedImplicitly]
    public static string OnConsoleCommand_event(string eventName, bool start)
    {
        GameEvent @event = Events.FirstOrDefault(e => e.Name.Equals(eventName, System.StringComparison.OrdinalIgnoreCase));
        if (!@event) return MessageHelpers.GetCommandOutput($"No event named '{eventName}'");

        if (start)
            @event.StartEvent();
        else
            @event.EndEvent();
        return MessageHelpers.GetCommandOutput($"{FormatStartEnd(start)}ed event {@event.GetType().Name}");
    }
}
