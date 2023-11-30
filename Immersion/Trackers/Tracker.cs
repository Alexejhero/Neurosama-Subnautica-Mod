using Nautilus.Utility;

namespace Immersion.Trackers;

/// <summary>
/// Base class for components that track the game's state and report events.
/// </summary>
/// <remarks>Subclasses should be <see langword="sealed" />.</remarks>
public abstract class Tracker : MonoBehaviour
{
    protected internal enum Priority
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

    /// <summary>
    /// Force the next <see cref="Send"/> or <see cref="React"/> call to proceed even if the component is disabled.
    /// </summary>
    public bool forceNext;

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
        if (!enabled && !forceNext) return;
        forceNext = false;

        _ = sender.Send(path, data);
    }

    protected internal void React(Priority priority, object data = null)
    {
        if (!enabled && !forceNext) return;
        forceNext = false;

        string endpoint = PickEndpoint(priority);
        if (endpoint == null) return;

        _ = sender.Send(endpoint, data);
    }

    internal static string PickEndpoint(Priority priority)
        => priority switch
        {
            Priority.Low => "non-priority",
            Priority.High => "priority",
            _ => null,
        };
}
