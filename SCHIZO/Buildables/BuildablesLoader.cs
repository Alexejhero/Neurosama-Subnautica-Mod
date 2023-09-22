using System.Diagnostics.CodeAnalysis;
using Nautilus.Crafting;
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
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    private static void Load()
	{
		DisabledParent = new GameObject("SCHIZO DISABLED PARENT").transform;
		DisabledParent.gameObject.SetActive(false);
		Object.DontDestroyOnLoad(DisabledParent);

        LoadOldVersions();

        new BuildablePrefab(new ModItem("VedalTurtle4", "Fake tutel", "that's crazy\n<size=75%>(Model by FutabaKuuhaku)</size>"))
        {
            IconFileName = "tutel_creature.png",
            Recipe = new RecipeData(new Ingredient(TechType.CreepvinePiece, 10), new Ingredient(ModItems.Tutel, 1)),
            PrefabName = "fake_tutel",
            TechGroup = TechGroup.Miscellaneous,
            TechCategory = TechCategory.Misc,
            RequiredForUnlock = ModItems.Tutel,
            ModifyPrefab = prefab =>
            {
                WorldSoundPlayer.Add(prefab, TutelWorldSounds);
            }
        }.WithOldVersion("VedalTurtle3").Register();

		new BuildablePrefab(ModItems.Erm)
		{
			IconFileName = "erm.png",
			Recipe = new RecipeData(new Ingredient(TechType.CopperWire, 2), new Ingredient(TechType.Silicone, 2), new Ingredient(TechType.Battery, 1), new Ingredient(TechType.Titanium, 4), new Ingredient(ModItems.Ermfish, 1)),
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
			Recipe = new RecipeData(new Ingredient(TechType.CopperWire, 1), new Ingredient(TechType.Silicone, 2), new Ingredient(Retargeting.TechType.Peeper, 2), new Ingredient(TechType.Bladderfish, 1)),
			PrefabName = "neurooper",
			TechGroup = TechGroup.Miscellaneous,
			TechCategory = TechCategory.Misc,
		}.WithOldVersion("Neurooper").Register();

		new BuildablePrefab(new ModItem("NeuroFumo2", "Low-poly Neuro fumo", "Fumo collection 1/2\n<size=75%>(Model by YuG)</size>"))
		{
			IconFileName = "neurofumo.png",
			Recipe = new RecipeData(new Ingredient(TechType.CopperWire, 1), new Ingredient(TechType.Silicone, 2), new Ingredient(TechType.JeweledDiskPiece, 1), new Ingredient(TechType.Gold, 1)),
			PrefabName = "neurofumo",
			TechGroup = TechGroup.Miscellaneous,
			TechCategory = TechCategory.Misc,
		}.WithOldVersion("NeuroFumo").Register();

		new BuildablePrefab(new ModItem("NeuroFumoNew", "Neuro fumo", "Fumo collection 2/2\n<size=75%>(Model by Kat)</size>"))
		{
			IconFileName = "neurofumonew.png",
			Recipe = new RecipeData(new Ingredient(TechType.CopperWire, 1), new Ingredient(TechType.Silicone, 2), new Ingredient(TechType.JeweledDiskPiece, 1), new Ingredient(TechType.Gold, 1)),
			PrefabName = "neurofumo2",
			TechGroup = TechGroup.Miscellaneous,
			TechCategory = TechCategory.Misc,
		}.Register();
	}

    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    private static void LoadOldVersions()
    {
        new BuildablePrefab(new ModItem("NeuroErm", "Erm (OLD VERSION, PLEASE REBUILD)", "<size=75%>(Model by w1n7er)</size>"))
        {
            Recipe = new RecipeData(new Ingredient(TechType.CopperWire, 2), new Ingredient(TechType.Silicone, 2), new Ingredient(TechType.Battery, 1), new Ingredient(TechType.Titanium, 4)),
            PrefabName = "neuroerm",
        }.Register();

        new BuildablePrefab(new ModItem("VedalTurtle2", "Fake tutel (OLD VERSION, PLEASE REBUILD)", "that's crazy\n<size=75%>(Model by FutabaKuuhaku)</size>"))
        {
            Recipe = new RecipeData(new Ingredient(TechType.CreepvinePiece, 10)),
            PrefabName = "fake_tutel",
        }.WithOldVersion("VedalTurtle").Register();
    }
}
