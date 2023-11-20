global using static Immersion.Plugin;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Immersion.Trackers;

namespace Immersion;

[BepInPlugin(PLUGIN_NAME, PLUGIN_NAME, "1.0.0")]
// without patches from the other mod, we can't register our console command
[BepInDependency("SCHIZO", BepInDependency.DependencyFlags.SoftDependency)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string PLUGIN_NAME = "Immersion";
    public static Assembly PLUGIN_ASSEMBLY { get; private set; }
    public static GameObject PLUGIN_OBJECT { get; private set; }
    public static GameObject COMPONENT_HOLDER { get; private set; }
    public static ManualLogSource LOGGER { get; private set; }
    public static Harmony HARMONY { get; private set; }

    private void Awake()
    {
        PLUGIN_ASSEMBLY = Assembly.GetExecutingAssembly();
        PLUGIN_OBJECT = gameObject;
        LOGGER = Logger;
        HARMONY = new Harmony(PLUGIN_NAME);
        HARMONY.PatchAll();
    }

    private void Start()
    {
        GameObject obj = new(PLUGIN_NAME);
        obj.transform.parent = PLUGIN_OBJECT.transform;
        COMPONENT_HOLDER = obj;
        obj.AddComponent<Sender>();
        foreach (Type trackerType in Tracker.trackerTypes.Values)
            obj.AddComponent(trackerType);
        obj.AddComponent<ConsoleCommands>();
    }
}
