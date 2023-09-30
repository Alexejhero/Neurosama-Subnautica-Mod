using JetBrains.Annotations;
using Nautilus.Commands;
using Nautilus.Utility;
using SCHIZO.Attributes;
using SCHIZO.Helpers;

namespace SCHIZO.Events;

[LoadConsoleCommands]
public sealed class CustomEventManager : MonoBehaviour
{
    public static CustomEventManager Instance;

    private readonly Dictionary<string, Type> Events = new(StringComparer.InvariantCultureIgnoreCase);

    private const string AUTOEVENTS_PREFS_KEY = "SCHIZO_Events_enableAutoEvents";
    public bool EnableAutoEvents
    {
        get => PlayerPrefsExtra.GetBool(AUTOEVENTS_PREFS_KEY, true);
        set => PlayerPrefsExtra.SetBool(AUTOEVENTS_PREFS_KEY, value);
    }

    public void Awake()
    {
        Instance = this;
    }

    public CustomEvent GetEvent(string eventName)
    {
        if (eventName is null)
            throw new ArgumentNullException(nameof(eventName));
        if (Events.TryGetValue(eventName, out Type eventType)
            || Events.TryGetValue(eventName+"Event", out eventType))
            return gameObject.GetComponent(eventType) as CustomEvent;
        return null;
    }

    public T GetEvent<T>() where T : CustomEvent
        => gameObject.GetComponent<T>();

    public void AddEvent<T>() where T : CustomEvent, new()
    {
        string eventName = typeof(T).Name;
        if (Events.ContainsKey(eventName))
        {
            LOGGER.LogWarning($"Event {eventName} already registered");
            return;
        }
        Type evt = typeof(T);
        Events.Add(eventName, evt);
        gameObject.EnsureComponent(evt);
    }

    public void RemoveEvent(string eventName)
    {
        if (Events.TryGetValue(eventName, out Type eventType))
        {
            Events.Remove(eventName);
            Component comp = gameObject.GetComponent(eventType);
            Destroy(comp);
        }
    }

    [ConsoleCommand("event"), UsedImplicitly]
    public static string OnConsoleCommand_event(string eventName, bool isStart)
    {
        CustomEventManager cem = Instance;

        if (cem.GetEvent(eventName) is not CustomEvent evt)
        {
            return Output($"Event '{eventName}' not found");
        }

        if (evt.IsOccurring == isStart)
        {
            return Output($"Event '{eventName}' is already {(evt.IsOccurring ? "" : "not ")}occurring");
        }

        if (isStart) evt.StartEvent();
        else evt.EndEvent();

        return Output($"Event '{eventName}' {(isStart ? "started" : "ended")}");
    }

    [ConsoleCommand("autoevents"), UsedImplicitly]
    public static string OnConsoleCommand_autoevents(bool enable)
    {
        Instance.EnableAutoEvents = enable;
        return Output($"Events are now {(enable ? "automatic" : "manual")}");
    }

    private static string Output(string msg) => MessageHelpers.GetCommandOutput(msg);
}
