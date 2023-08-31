using Nautilus.Crafting;
using SCHIZO.Creatures.Ermfish;
using SCHIZO.Utilities;
using UnityEngine;

namespace SCHIZO.Buildables;

public static class BuildablesLoader
{
	public static Transform DisabledParent { get; private set; }

	public static LocalSoundCollection ErmWorldSounds { get; private set; }

	public static void Load()
	{
		DisabledParent = new GameObject("SCHIZO DISABLED PARENT").transform;
		DisabledParent.gameObject.SetActive(false);
		GameObject.DontDestroyOnLoad(DisabledParent);

		ErmWorldSounds = new LocalSoundCollection("noises", "bus:/master/SFX_for_pause/PDA_pause/all/indoorsounds");

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

		new BuildablePrefab("NeuroErm", "Erm (OLD, PLEASE REBUILD)", "<size=75%>(Model by w1n7er)</size>")
		{
			AssetBundleName = "erm",
			IconFileName = "erm.png",
			Recipe = new RecipeData(new CraftData.Ingredient(TechType.CopperWire, 2), new CraftData.Ingredient(TechType.Silicone, 2), new CraftData.Ingredient(TechType.Battery), new CraftData.Ingredient(TechType.Titanium, 4)),
			PrefabName = "erm_fishes",
			PrefabScaleMultiplier = 0.15f,
			ModifyPrefab = prefab =>
			{
				prefab.transform.GetChild(0).localScale = new Vector3(1, -1, 1);
				prefab.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
				prefab.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
				prefab.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
				prefab.GetComponentsInChildren<MeshRenderer>().ForEach(r => r.material.color = new Color(0.75f, 0, 0.75f));
			}
		}.Register();

		new BuildablePrefab("NeuroErm2", "Erm", "<size=75%>(Model by w1n7er)</size>")
		{
			AssetBundleName = "erm",
			IconFileName = "erm.png",
			Recipe = new RecipeData(new CraftData.Ingredient(TechType.CopperWire, 2), new CraftData.Ingredient(TechType.Silicone, 2), new CraftData.Ingredient(TechType.Battery), new CraftData.Ingredient(TechType.Titanium, 4), new CraftData.Ingredient(ErmfishTypes.Regular.TechType)),
			PrefabName = "erm_fishes",
			TechGroup = TechGroup.Miscellaneous,
			TechCategory = TechCategory.Misc,
			PrefabScaleMultiplier = 0.15f,
			ModifyPrefab = prefab =>
			{
				prefab.AddComponent<ErmNoises>();
				prefab.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
				prefab.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
			}
		}.Register();

		new BuildablePrefab("NeuroFumo", "Low-poly Neuro fumo", "Fumo collection 1/2\n<size=75%>(Model by YuG)</size>")
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

		new BuildablePrefab("NeuroFumo2", "Neuro fumo", "Fumo collection 2/2\n<size=75%>(Model by Kat)</size>")
		{
			AssetBundleName = "neurofumo2",
			IconFileName = "neurofumo2.png",
			Recipe = new RecipeData(new CraftData.Ingredient(TechType.CopperWire), new CraftData.Ingredient(TechType.Silicone, 2), new CraftData.Ingredient(TechType.JeweledDiskPiece, 2), new CraftData.Ingredient(TechType.Gold)),
			PrefabName = "neurofumo2",
			TechGroup = TechGroup.Miscellaneous,
			TechCategory = TechCategory.Misc,
			PrefabScaleMultiplier = 0.1f,
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
