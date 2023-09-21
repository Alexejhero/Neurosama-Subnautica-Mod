global using static SCHIZO.Plugin;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using SCHIZO.Attributes;
using SCHIZO.Resources;

namespace SCHIZO;

[BepInPlugin("SCHIZO", "Neuro-sama Mod", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
#if SUBNAUTICA
    public static bool IS_SUBNAUTICA => true;
    public static bool IS_BELOWZERO => false;
#else
    public static bool IS_SUBNAUTICA => false;
    public static bool IS_BELOWZERO => true;
#endif

    public static ManualLogSource LOGGER { get; private set; }

    public static readonly Config CONFIG = OptionsPanelHandler.RegisterModOptions<Config>();

	private void Awake()
	{
        LOGGER = Logger;
        DependencyResolver.InjectResources();

        LoadMethodAttribute.LoadAll();
        LoadComponentAttribute.AddAll(gameObject);
        LoadConsoleCommandsAttribute.RegisterAll();

		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
	}
}
