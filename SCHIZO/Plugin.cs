global using static SCHIZO.Plugin;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using SCHIZO.Buildables;
using SCHIZO.Creatures.Ermfish;
using SCHIZO.Creatures.Ermshark;
using SCHIZO.Gymbag;
using SCHIZO.HullPlates;
using SCHIZO.Twitch;

namespace SCHIZO;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    private static ManualLogSource _logger;
    public static ManualLogSource LOGGER => _logger;
	public static readonly Config CONFIG = OptionsPanelHandler.RegisterModOptions<Config>();

	private void Awake()
	{
        _logger = Logger;
        HullPlateLoader.Load();
		BuildablesLoader.Load();
		ErmfishLoader.Load();
		ErmsharkLoader.Load();
		GymbagLoader.Load();

        gameObject.AddComponent<TwitchIntegration>();

		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        CraftDataHandler.SetCraftingTime(TechType.TitaniumIngot, 600);
	}
}
