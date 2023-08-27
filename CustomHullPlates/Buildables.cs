using System.IO;
using System.Reflection;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace SCHIZO
{
    public static class Buildables
    {
	    private static AssetBundle turtleBundle = AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly.GetExecutingAssembly(), "turtle");
	    private static AssetBundle fumoBundle = AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly.GetExecutingAssembly(), "neurofumo");
	    private static AssetBundle operBundle = AssetBundleLoadingUtils.LoadFromAssetsFolder(Assembly.GetExecutingAssembly(), "neurooper");

        public static void Load()
        {
	        LoadTurtle();
	        LoadErm();
	        LoadNeuroFumo();
	        LoadNeurooper();
        }

        private static void LoadTurtle()
		{
            var turtle = new CustomPrefab("VedalTurtle", "Tutel", "<size=75%>(Model by FutabaKuuhaku)</size>");

			turtle.SetGameObject(GetPrefab);
			turtle.Info.WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(SchizoPlugin.assetsFolder, "tutel.png")));

			turtle.SetPdaGroupCategory(TechGroup.Miscellaneous, TechCategory.Misc);
			turtle.SetRecipe(new RecipeData(new CraftData.Ingredient(TechType.CreepvinePiece, 10)));

			turtle.Register();

			GameObject GetPrefab()
			{
				var prefab = turtleBundle.LoadAsset<GameObject>("turtle2");
				PrefabUtils.AddBasicComponents(prefab, turtle.Info.ClassID, turtle.Info.TechType, LargeWorldEntity.CellLevel.Far);
				var con = PrefabUtils.AddConstructable(prefab, turtle.Info.TechType, ConstructableFlags.Outside | ConstructableFlags.Base | ConstructableFlags.Submarine | ConstructableFlags.AllowedOnConstructable | ConstructableFlags.Ground | ConstructableFlags.Inside, prefab.FindChild("turtle"));

				MaterialUtils.ApplySNShaders(prefab, 1);
				prefab.transform.localScale *= 2f;
				con.rotationEnabled = true;

				return prefab;
			}
		}

		private static void LoadNeuroFumo()
		{
			var customPrefab = new CustomPrefab("NeuroFumo", "Neuro fumo", "Just don't turn it around Clueless\n<size=75%>(Model by YuG)</size>");

			customPrefab.SetGameObject(GetPrefab);
			customPrefab.Info.WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(SchizoPlugin.assetsFolder, "neurofumo.png")));

			customPrefab.SetPdaGroupCategory(TechGroup.Miscellaneous, TechCategory.Misc);
			customPrefab.SetRecipe(new RecipeData(new CraftData.Ingredient(TechType.CopperWire, 1), new CraftData.Ingredient(TechType.Silicone, 2), new CraftData.Ingredient(TechType.JeweledDiskPiece, 2), new CraftData.Ingredient(TechType.Gold)));

			customPrefab.Register();

			GameObject GetPrefab()
			{
				var prefab = fumoBundle.LoadAsset<GameObject>("neurofuno");
				PrefabUtils.AddBasicComponents(prefab, customPrefab.Info.ClassID, customPrefab.Info.TechType, LargeWorldEntity.CellLevel.Far);
				var child = prefab.transform.GetChild(0).gameObject;
				child.transform.Rotate(0, 180, 180);
				var con = PrefabUtils.AddConstructable(prefab, customPrefab.Info.TechType, ConstructableFlags.Outside | ConstructableFlags.Base | ConstructableFlags.Submarine | ConstructableFlags.AllowedOnConstructable | ConstructableFlags.Ground | ConstructableFlags.Inside, child);

				MaterialUtils.ApplySNShaders(prefab, 1);
				prefab.transform.localScale *= 0.3f;
				con.rotationEnabled = true;

				return prefab;
			}
		}

		private static void LoadErm()
		{
			var customPrefab = new CustomPrefab("NeuroErm", "Erm", "<size=75%>(Model by w1n7er)</size>");

			customPrefab.SetGameObject(GetPrefab);
			customPrefab.Info.WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(SchizoPlugin.assetsFolder, "erm.png")));

			customPrefab.SetPdaGroupCategory(TechGroup.Miscellaneous, TechCategory.Misc);
			customPrefab.SetRecipe(new RecipeData(new CraftData.Ingredient(TechType.CopperWire, 2), new CraftData.Ingredient(TechType.Silicone, 2), new CraftData.Ingredient(TechType.Battery), new CraftData.Ingredient(TechType.Titanium, 4)));

			customPrefab.Register();

			GameObject GetPrefab()
			{
				var prefab = SchizoPlugin.ermBundle.LoadAsset<GameObject>("erm_fishes");
				prefab.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
				prefab.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
				PrefabUtils.AddBasicComponents(prefab, customPrefab.Info.ClassID, customPrefab.Info.TechType, LargeWorldEntity.CellLevel.Far);
				var con = PrefabUtils.AddConstructable(prefab, customPrefab.Info.TechType, ConstructableFlags.Outside | ConstructableFlags.Base | ConstructableFlags.Submarine | ConstructableFlags.AllowedOnConstructable | ConstructableFlags.Ground | ConstructableFlags.Inside, prefab.FindChild("erm"));

				MaterialUtils.ApplySNShaders(prefab, 1);
				prefab.transform.localScale *= 0.15f;
				con.rotationEnabled = true;

				return prefab;
			}
		}

		private static void LoadNeurooper()
		{
			var customPrefab = new CustomPrefab("Neuroopper", "Neurooper", "<size=75%>(Model by greencap, original art by Sandro)</size>");

			customPrefab.SetGameObject(GetPrefab);
			customPrefab.Info.WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(SchizoPlugin.assetsFolder, "neurooper.png")));

			customPrefab.SetPdaGroupCategory(TechGroup.Miscellaneous, TechCategory.Misc);
			customPrefab.SetRecipe(new RecipeData(new CraftData.Ingredient(TechType.CopperWire, 1), new CraftData.Ingredient(TechType.Silicone, 2), new CraftData.Ingredient(TechType.Peeper, 3), new CraftData.Ingredient(TechType.Bladderfish, 3)));

			customPrefab.Register();

			GameObject GetPrefab()
			{
				var prefab = operBundle.LoadAsset<GameObject>("neuiropper");
				PrefabUtils.AddBasicComponents(prefab, customPrefab.Info.ClassID, customPrefab.Info.TechType, LargeWorldEntity.CellLevel.Far);
				var con = PrefabUtils.AddConstructable(prefab, customPrefab.Info.TechType, ConstructableFlags.Outside | ConstructableFlags.Base | ConstructableFlags.Submarine | ConstructableFlags.AllowedOnConstructable | ConstructableFlags.Ground | ConstructableFlags.Inside, prefab.FindChild("neurooper"));

				MaterialUtils.ApplySNShaders(prefab, 1);
				// prefab.transform.localScale *= 0.15f;
				con.rotationEnabled = true;

				return prefab;
			}
		}
    }
}
