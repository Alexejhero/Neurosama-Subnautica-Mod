using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SCHIZO.Commands;
using SCHIZO.Commands.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;

namespace SCHIZO.Events;

[CommandCategory("Game Events")]
public partial class GameEventsManager
{
    public static bool AutoStart
    {
        get => Assets.Mod_Options_EnableAutomaticEvents.Value;
        set => Assets.Mod_Options_EnableAutomaticEvents.Value = value;
    }

    private static GameEventsManager Instance { get; set; }
    private static List<GameEvent> Events { get; set; } = [];

    private void Awake()
    {
        if (Instance) Destroy(Instance);
        Instance = this;
    }
    private void Start()
    {
        gameObject.GetComponents(Events);
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

    [Command(Name = "autoevents",
        DisplayName = "Auto Event Start",
        Description = "Get or set whether events start automatically",
        RegisterConsoleCommand = true)]
    public static string AutoEvents(bool? arg = null)
    {
        if (arg is not { } value)
            return $"Events are currently {FormatAutoStart(AutoStart)}";

        AutoStart = value;

        return MessageHelpers.GetCommandOutput($"Events are now {FormatAutoStart(arg.Value)}");
    }

    [Command(Name = "event",
        DisplayName = "Toggle Event",
        Description = "Start or stop a specific game event",
        RegisterConsoleCommand = true)]
    public static string Event(string eventName, bool start)
    {
        GameEvent @event = Events.Find(e => e.EventName.Equals(eventName, System.StringComparison.OrdinalIgnoreCase));
        if (!@event) return MessageHelpers.GetCommandOutput($"No event named '{eventName}'");

        if (start)
            @event.StartEvent();
        else
            @event.EndEvent();
        return MessageHelpers.GetCommandOutput($"{FormatStartEnd(start)}ed event {@event.GetType().Name}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string FormatAutoStart(bool value) => value ? "automatic" : "manual";
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string FormatStartEnd(bool start) => start ? "start" : "end";
}
