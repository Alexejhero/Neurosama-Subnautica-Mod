﻿using Nautilus.Crafting;
using SCHIZO.Attributes;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Buildables;

[LoadMethod]
public static class BuildablesLoader
{
	public static Transform DisabledParent { get; private set; }

    private const string INDOOR_SOUNDS_BUS = "bus:/master/SFX_for_pause/PDA_pause/all/indoorsounds";

	private static readonly SoundCollection ErmWorldSounds = SoundCollection.Create("ermfish/noises", INDOOR_SOUNDS_BUS);
    private static readonly SoundCollection TutelWorldSounds = SoundCollection.Create("tutel/noises", INDOOR_SOUNDS_BUS);

    [LoadMethod]
	private static void Load()
	{
		DisabledParent = new GameObject("SCHIZO DISABLED PARENT").transform;
		DisabledParent.gameObject.SetActive(false);
		GameObject.DontDestroyOnLoad(DisabledParent);

		LoadOldVersions();

        new BuildablePrefab(new ModItem("VedalTurtle3", "Fake tutel", "that's crazy\n<size=75%>(Model by FutabaKuuhaku)</size>"))
        {
            IconFileName = "tutel.png",
            Recipe = new RecipeData(new CraftData.Ingredient(TechType.CreepvinePiece, 10), new CraftData.Ingredient(ModItems.Tutel)),
            PrefabName = "turtle",
            TechGroup = TechGroup.Miscellaneous,
            TechCategory = TechCategory.Misc,
            ModifyPrefab = prefab =>
            {
                WorldSoundPlayer.Add(prefab, TutelWorldSounds);
            }
        }.Register();

		new BuildablePrefab(ModItems.Erm)
		{
			IconFileName = "erm.png",
			Recipe = new RecipeData(new CraftData.Ingredient(TechType.CopperWire, 2), new CraftData.Ingredient(TechType.Silicone, 2), new CraftData.Ingredient(TechType.Battery), new CraftData.Ingredient(TechType.Titanium, 4), new CraftData.Ingredient(ModItems.Ermfish)),
			PrefabName = "neuroerm",
			TechGroup = TechGroup.Miscellaneous,
			TechCategory = TechCategory.Misc,
            RequiredForUnlock = ModItems.Ermfish,
			ModifyPrefab = prefab =>
			{
				WorldSoundPlayer.Add(prefab, ErmWorldSounds);
			}
		}.Register();

		new BuildablePrefab(new ModItem("Neuroopper2", "Neurooper", "<size=75%>(Model by greencap, original art by Sandro)</size>"))
		{
			IconFileName = "neurooper.png",
			Recipe = new RecipeData(new CraftData.Ingredient(TechType.CopperWire), new CraftData.Ingredient(TechType.Silicone, 2), new CraftData.Ingredient(TechType.Peeper, 3), new CraftData.Ingredient(TechType.Bladderfish, 3)),
			PrefabName = "neurooper",
			TechGroup = TechGroup.Miscellaneous,
			TechCategory = TechCategory.Misc,
		}.WithOldVersion("Neurooper").Register();

		new BuildablePrefab(new ModItem("NeuroFumo2", "Low-poly Neuro fumo", "Fumo collection 1/2\n<size=75%>(Model by YuG)</size>"))
		{
			IconFileName = "neurofumo.png",
			Recipe = new RecipeData(new CraftData.Ingredient(TechType.CopperWire), new CraftData.Ingredient(TechType.Silicone, 2), new CraftData.Ingredient(TechType.JeweledDiskPiece, 2), new CraftData.Ingredient(TechType.Gold)),
			PrefabName = "neurofumo",
			TechGroup = TechGroup.Miscellaneous,
			TechCategory = TechCategory.Misc,
		}.WithOldVersion("NeuroFumo").Register();

		new BuildablePrefab(new ModItem("NeuroFumoNew", "Neuro fumo", "Fumo collection 2/2\n<size=75%>(Model by Kat)</size>"))
		{
			IconFileName = "neurofumonew.png",
			Recipe = new RecipeData(new CraftData.Ingredient(TechType.CopperWire), new CraftData.Ingredient(TechType.Silicone, 2), new CraftData.Ingredient(TechType.JeweledDiskPiece, 2), new CraftData.Ingredient(TechType.Gold)),
			PrefabName = "neurofumo2",
			TechGroup = TechGroup.Miscellaneous,
			TechCategory = TechCategory.Misc,
		}.Register();
	}

	private static void LoadOldVersions()
	{
		new BuildablePrefab(new ModItem("NeuroErm", "Erm (OLD VERSION, PLEASE REBUILD)", "<size=75%>(Model by w1n7er)</size>"))
		{
			IconFileName = "erm.png",
			Recipe = new RecipeData(new CraftData.Ingredient(TechType.CopperWire, 2), new CraftData.Ingredient(TechType.Silicone, 2), new CraftData.Ingredient(TechType.Battery), new CraftData.Ingredient(TechType.Titanium, 4)),
			PrefabName = "neuroerm",
			ModifyPrefab = prefab =>
			{
				prefab.transform.Find("WM").localScale = new Vector3(1, -1, 1);
				prefab.transform.Find("WM/erm/regular").gameObject.SetActive(false);
				prefab.transform.Find("WM/erm/cured").gameObject.SetActive(true);
				prefab.GetComponentsInChildren<MeshRenderer>().ForEach(r => r.material.color = new Color(0.75f, 0, 0.75f));
			}
		}.Register();

        new BuildablePrefab(new ModItem("VedalTurtle2", "Fake tutel (OLD VERSION, PLEASE REBUILD)", "that's crazy\n<size=75%>(Model by FutabaKuuhaku)</size>"))
        {
            IconFileName = "tutel.png",
            Recipe = new RecipeData(new CraftData.Ingredient(TechType.CreepvinePiece, 10)),
            PrefabName = "turtle",
        }.WithOldVersion("VedalTurtle").Register();
	}
}
