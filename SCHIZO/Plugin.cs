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

namespace SCHIZO;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
	public static readonly ManualLogSource LOGGER = BepInEx.Logging.Logger.CreateLogSource("SCHIZO"); // TODO: move old logs to this
	public static readonly Config CONFIG = OptionsPanelHandler.RegisterModOptions<Config>();

	private void Awake()
	{
        HullPlateLoader.Load();
		BuildablesLoader.Load();
		ErmfishLoader.Load();
		ErmsharkLoader.Load();
		GymbagLoader.Load();

		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
	}
}
