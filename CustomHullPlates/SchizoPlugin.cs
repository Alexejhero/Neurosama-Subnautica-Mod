using System.IO;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace SCHIZO
{
	// Token: 0x02000002 RID: 2
	[BepInPlugin("SCHIZOMOD", "SCHIZO MOD", "1.0.0.0")]
	public class SchizoPlugin : BaseUnityPlugin
	{
		// ReSharper disable once AssignNullToNotNullAttribute
		public static string assetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
		public static AssetBundle ermBundle = AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly.GetExecutingAssembly(), "erm");

		public static Config config = OptionsPanelHandler.RegisterModOptions<Config>();

		private void Awake()
		{
			HullPlates.Load();
			Buildables.Load();
            Ermfish.Load();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
		}
    }
}
