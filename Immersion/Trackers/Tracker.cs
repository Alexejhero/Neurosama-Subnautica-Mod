using HarmonyLib;
using Nautilus.Utility;

namespace Immersion.Trackers;

public abstract class Tracker : MonoBehaviour
{
    protected enum Priority
    {
        Low,
        High,
    }

    internal static readonly Dictionary<string, Type> trackerTypes;
    private string startEnabledPrefsKey => $"{GetType().FullName}_Enabled";
    internal bool startEnabled
    {
        get => PlayerPrefsExtra.GetBool(startEnabledPrefsKey, true);
        set => PlayerPrefsExtra.SetBool(startEnabledPrefsKey, value);
    }

    static Tracker()
    {
        // all trackers should (1) inherit directly from Tracker, and (2) be sealed
        trackerTypes = PLUGIN_ASSEMBLY.GetTypes()
            .Where(t => t.BaseType == typeof(Tracker))
            .ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);
    }

    internal Sender sender;

    protected virtual void Awake()
    {
        sender = GetComponent<Sender>();
        enabled = startEnabled;
    }

    protected void Send(string path, object data = null)
    {
        if (!enabled) return;

        _ = sender.Send(path, data);
    }

    protected void React(Priority priority, object data = null)
    {
        if (!enabled) return;

        string endpoint = PickEndpoint(priority);
        if (endpoint == null) return;

        _ = sender.Send(endpoint, data);
    }

    private string PickEndpoint(Priority priority)
        => priority switch
        {
            Priority.Low => "non-priority",
            Priority.High => "priority",
            _ => null,
        };
}
