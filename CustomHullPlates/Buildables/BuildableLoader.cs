using Nautilus.Crafting;
using Vector3 = System.Numerics.Vector3;

namespace SCHIZO.Buildables;

public static class BuildableLoader
{
	public static void Load()
	{
		new BuildablePrefab("VedalTurtle", "Tutel", "<size=75%>(Model by FutabaKuuhaku)</size>")
		{
			AssetBundleName = "turtle",
			IconFileName = "tutel.png",
			Recipe = new RecipeData(new CraftData.Ingredient(TechType.CreepvinePiece, 10)),
			PrefabName = "turtle2",
			TechGroup = TechGroup.Miscellaneous,
			TechCategory = TechCategory.Misc,
			PrefabScaleMultiplier = 2,
		}.Register();

		new BuildablePrefab("NeuroErm", "Erm", "<size=75%>(Model by w1n7er)</size>")
		{
			AssetBundleName = "erm",
			IconFileName = "erm.png",
			Recipe = new RecipeData(new CraftData.Ingredient(TechType.CopperWire, 2), new CraftData.Ingredient(TechType.Silicone, 2), new CraftData.Ingredient(TechType.Battery), new CraftData.Ingredient(TechType.Titanium, 4)),
			PrefabName = "erm_fishes",
			TechGroup = TechGroup.Miscellaneous,
			TechCategory = TechCategory.Misc,
			PrefabScaleMultiplier = 0.15f,
			ModifyPrefab = prefab =>
			{
				prefab.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
				prefab.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
			}
		}.Register();

		new BuildablePrefab("NeuroFumo", "Neuro fumo", "<size=75%>(Model by YuG)</size>")
		{
			AssetBundleName = "neurofumo",
			IconFileName = "neurofumo.png",
			Recipe = new RecipeData(new CraftData.Ingredient(TechType.CopperWire), new CraftData.Ingredient(TechType.Silicone, 2), new CraftData.Ingredient(TechType.JeweledDiskPiece, 2), new CraftData.Ingredient(TechType.Gold)),
			PrefabName = "neurofuno",
			TechGroup = TechGroup.Miscellaneous,
			TechCategory = TechCategory.Misc,
			PrefabRotationEuler = new Vector3(0, 180, 180),
			PrefabScaleMultiplier = 0.3f,
		}.Register();

		new BuildablePrefab("Neuroopper", "Neurooper", "<size=75%>(Model by greencap, original art by Sandro)</size>")
		{
			AssetBundleName = "neurooper",
			IconFileName = "neurooper.png",
			Recipe = new RecipeData(new CraftData.Ingredient(TechType.CopperWire), new CraftData.Ingredient(TechType.Silicone, 2), new CraftData.Ingredient(TechType.Peeper, 3), new CraftData.Ingredient(TechType.Bladderfish, 3)),
			PrefabName = "neuiropper",
			TechGroup = TechGroup.Miscellaneous,
			TechCategory = TechCategory.Misc,
		}.Register();
	}
}
