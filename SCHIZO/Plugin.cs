using System.Reflection;
using BepInEx;
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
	public new static readonly Config Config = OptionsPanelHandler.RegisterModOptions<Config>();

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
