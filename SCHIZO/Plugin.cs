global using static SCHIZO.Plugin;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using SCHIZO.Buildables;
using SCHIZO.Creatures.Ermfish;
using SCHIZO.Creatures.Ermshark;
using SCHIZO.Greggs;
using SCHIZO.Gymbag;
using SCHIZO.HullPlates;
using SCHIZO.Resources;
using SCHIZO.Twitch;

namespace SCHIZO;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    public static ManualLogSource LOGGER { get; private set; }

    public static readonly Config CONFIG = OptionsPanelHandler.RegisterModOptions<Config>();

	private void Awake()
	{
        LOGGER = Logger;
        DependencyResolver.InjectResources();

        HullPlateLoader.Load();
		BuildablesLoader.Load();
		ErmfishLoader.Load();
		ErmsharkLoader.Load();
		GymbagLoader.Load();
        GreggsLoader.Load();

        gameObject.AddComponent<TwitchIntegration>();

		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
	}
}
