using Nautilus.Commands;
using Nautilus.Utility;
using SCHIZO.Attributes.Loading;
using UnityEngine;

namespace SCHIZO.Events;

[LoadConsoleCommands]
public partial class GameEventsConfig
{
    public static GameEventsConfig instance;
    public static GameObject manager;
    private void Awake()
    {
        instance = this;
        manager = gameObject;
        if (PlayerPrefs.GetInt(AUTOEVENTS_PREFS_KEY, -1) != -1)
            HasBeenSet = true;
        else
            SetAutoStart(autoStartEvents);
    }
    private const string AUTOEVENTS_PREFS_KEY = "SCHIZO_Events_AutoEvents";
    public static bool HasBeenSet { get; private set; }
    public bool AutoStartEvents
    {
        get => GetAutoStart(autoStartEvents);
        set => SetAutoStart(value);
    }
    private static bool GetAutoStart(bool defaultVal = true)
        => PlayerPrefsExtra.GetBool(AUTOEVENTS_PREFS_KEY, defaultVal);
    private static void SetAutoStart(bool value)
    {
        HasBeenSet = true;
        PlayerPrefsExtra.SetBool(AUTOEVENTS_PREFS_KEY, value);
    }

    [ConsoleCommand("autoevents")]
    private static string OnConsoleCommand_autoevents(bool? autostartEvents = null)
    {
        if (autostartEvents == null)
            return $"Events are currently {FormatAutoStart(GetAutoStart())}";
        SetAutoStart(autostartEvents.Value);
        return $"Events are now {FormatAutoStart(GetAutoStart())}";
    }

    private static string FormatAutoStart(bool value)
        => value ? "automatic" : "manual";
}
