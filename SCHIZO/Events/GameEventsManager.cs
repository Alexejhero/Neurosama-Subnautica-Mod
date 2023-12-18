using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Nautilus.Commands;
using SCHIZO.ConsoleCommands;
using SCHIZO.Helpers;
using SCHIZO.Resources;

namespace SCHIZO.Events;

[RegisterConsoleCommands]
public partial class GameEventsManager
{
    public static bool AutoStart
    {
        get => Assets.Mod_Options_EnableAutomaticEvents.Value;
        set => Assets.Mod_Options_EnableAutomaticEvents.Value = value;
    }

    private static GameEventsManager Instance { get; set; }
    private static List<GameEvent> Events { get; set; } = new();

    private void Awake()
    {
        if (Instance) Destroy(Instance);
        Instance = this;
    }
    private void Start()
    {
        gameObject.GetComponents(Events);
    }

    [ConsoleCommand("autoevents"), UsedImplicitly]
    public static string OnConsoleCommand_autoevents(bool? arg = null)
    {
        if (arg is not { } value)
        {
            return $"Events are currently {FormatAutoStart(AutoStart)}";
        }

        AutoStart = value;

        return MessageHelpers.GetCommandOutput($"Events are now {FormatAutoStart(arg.Value)}");
    }

#if DEBUG_ERMCON
    public void Update()
    {
        if (!Input.GetKeyDown(KeyCode.LeftControl)) return;
        if (!Player.main || !Player.main.guiHand || !Player.main.guiHand.activeTarget) return;

        ErmconAttendee target = Player.main.guiHand.activeTarget.GetComponent<ErmconAttendee>();
        if (!target) return;

        target.CycleDebug();
    }
#endif

    [ConsoleCommand("event"), UsedImplicitly]
    public static string OnConsoleCommand_event(string eventName, bool start)
    {
        GameEvent @event = Events.FirstOrDefault(e => e.EventName.Equals(eventName, System.StringComparison.OrdinalIgnoreCase));
        if (!@event) return MessageHelpers.GetCommandOutput($"No event named '{eventName}'");

        if (start) @event.StartEvent();
        else @event.EndEvent();
        return MessageHelpers.GetCommandOutput($"{FormatStartEnd(start)}ed event {@event.GetType().Name}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string FormatAutoStart(bool value) => value ? "automatic" : "manual";
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string FormatStartEnd(bool start) => start ? "start" : "end";
}
