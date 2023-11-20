global using static Immersion.Plugin;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Immersion.Trackers;

namespace Immersion;

[BepInPlugin(PLUGIN_NAME, PLUGIN_NAME, "1.0.0")]
public sealed class Plugin : BaseUnityPlugin
{
    public const string PLUGIN_NAME = "Immersion";
    public static Assembly PLUGIN_ASSEMBLY { get; private set; }
    public static GameObject PLUGIN_OBJECT { get; private set; }
    public static ManualLogSource LOGGER { get; private set; }
    public static Harmony HARMONY { get; private set; }

    private void Awake()
    {
        PLUGIN_ASSEMBLY = Assembly.GetExecutingAssembly();
        PLUGIN_OBJECT = gameObject;
        LOGGER = Logger;
        HARMONY = new Harmony("Immersion");
        HARMONY.PatchAll();
    }

    private void Start()
    {
        gameObject.AddComponent<Sender>();
        foreach (Type trackerType in Tracker.trackerTypes.Values)
            gameObject.AddComponent(trackerType);
        gameObject.AddComponent<ConsoleCommands>();
    }
}
