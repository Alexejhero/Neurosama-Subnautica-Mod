using Nautilus.Utility;

namespace Immersion;

public abstract class Tracker : MonoBehaviour
{
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

    public void Send(string path, object data = null)
    {
        if (!enabled) return;

        _ = sender.Send(path, data);
    }
}
