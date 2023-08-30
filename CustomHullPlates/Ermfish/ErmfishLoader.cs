using System;
using System.Collections.Generic;
using System.Linq;
using ECCLibrary;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace SCHIZO.Ermfish;

public static class ErmfishLoader
{
    public static SoundCollection CraftSounds { get; private set; }
    public static LocalSoundCollection DropSounds { get; private set; }
    public static SoundCollection EatSounds { get; private set; }
    public static SoundCollection EquipSounds { get; private set; }
    public static LocalSoundCollection HurtSounds { get; private set; }
    public static SoundCollection InventorySounds { get; private set; }
    public static SoundCollection PickupSounds { get; private set; }
    public static SoundCollection PlayerDeathSounds { get; private set; }
    public static SoundCollection ScanSounds { get; private set; }
    public static SoundCollection UnequipSounds { get; private set; }
    public static LocalSoundCollection WorldSounds { get; private set; } // todo: get rid of fmod erros for loading the same audio file twice

    public static void Load()
    {
        LoadSounds();
        LoadErmfish();
        LoadErmfishVariant(ErmfishTypes.Cooked, "erm_cooked.png", new RecipeData(new CraftData.Ingredient(ErmfishTypes.Regular.TechType)), 23, 4, true, "CookedFood", TechCategory.CookedFood, 2);
        LoadErmfishVariant(ErmfishTypes.Cured, "erm_cured.png", new RecipeData(new CraftData.Ingredient(ErmfishTypes.Regular.TechType), new CraftData.Ingredient(TechType.Salt)), 23, -2, false, "CuredFood", TechCategory.CuredFood, 1);
    }

    private static void LoadSounds()
    {
        CraftSounds = new SoundCollection("sounds/cooking", AudioUtils.BusPaths.PDAVoice);
        DropSounds = new LocalSoundCollection("sounds/release", AudioUtils.BusPaths.PDAVoice);
        EatSounds = new SoundCollection("sounds/eating", AudioUtils.BusPaths.PDAVoice);
        EquipSounds = new SoundCollection("sounds/equipping", AudioUtils.BusPaths.PDAVoice);
        HurtSounds = new LocalSoundCollection("sounds/hurt", AudioUtils.BusPaths.PDAVoice);
        InventorySounds = new SoundCollection("sounds/noises", AudioUtils.BusPaths.PDAVoice);
        PickupSounds = new SoundCollection("sounds/pickup", AudioUtils.BusPaths.PDAVoice);
        PlayerDeathSounds = new SoundCollection("sounds/player_death", "bus:/master/SFX_for_pause/nofilter");
        ScanSounds = new SoundCollection("sounds/scan", AudioUtils.BusPaths.PDAVoice);
        UnequipSounds = new SoundCollection("sounds/unequipping", AudioUtils.BusPaths.PDAVoice);
        WorldSounds = new LocalSoundCollection("sounds/noises", AudioUtils.BusPaths.UnderwaterCreatures);
    }

    private static void LoadErmfish()
    {
		ErmfishPrefab creature = new(ErmfishTypes.Regular);
		creature.PrefabInfo.WithIcon(AssetLoader.GetAtlasSprite("erm.png"));
		creature.Register();

		Texture2D databankTexture = AssetLoader.GetTexture("ermfish-databank.png");
		Texture2D unlockTexture = AssetLoader.GetTexture("ermfish-unlock.png");
		Sprite unlockSprite = Sprite.Create(unlockTexture, new Rect(0, 0, unlockTexture.width, unlockTexture.height), Vector2.zero);

		CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(creature, "Lifeforms/Fauna/SmallHerbivores", "Ermfish",
			"""
			An entity of unknown origin, it does not appear to be indigenous to 4546B. Although at first glance it appears to be an aquatic lifeform, it does not possess the necessary survival facilities.

			This species appears to consist mostly of a fibrous muscle mass, no internal organs can be located inside the creature. This unique biology may indicate its purpose as part of a larger organism instead of an individual. Attempts to euthanize, dissect, or damage the specimen have resulted in failure, as the creature presents unnatural healing abilities. As it stands, we aren't sure if even the heat death of the Universe could properly dispose of an Ermfish. It is presumed that the only way to destroy the creature is by assimilating it in a larger organism.

			1. Ears:
			The ears situated at the top of the Ermfish have no opening and appear to be a type of mobility organ for swimming or maintaining balance in the water.

			2. Antenna:
			Between the ears there is a single antenna-like organ that emits a faint radio-signal. This could indicate communication between the species or another entity altogether.

			3. Eyes:
			Unlike most living creatures, these elliptical protrusions are made out of a hard, opaque material. Optical examination of the supposed pupils determined them to be completely impenetrable to light, and it is a question as to how the Ermfish is capable of perceiving its surroundings. One hypothesis suggests these appendages serve a different function other than sight.

			Being in the vicinity of an Ermfish may cause auditory hallucinations that cannot be reproduced on audio recordings. The effect is magnified proportionally to the number of Ermfish present. Long-term effects are uncertain, but it is speculated that it may cause irreversible damage to the exposed individual.

			Assessment: Experimental results have shown that Ermfish is technically suitable for human consumption. However, high mental fortitude is required to go to such desperate lengths.
			""", 5, databankTexture, unlockSprite);

		List<LootDistributionData.BiomeData> biomes = new();
		foreach (BiomeType biome in Enum.GetValues(typeof(BiomeType)).Cast<BiomeType>())
		{
			biomes.Add(new LootDistributionData.BiomeData { biome = biome, count = 1, probability = 0.1f });
			biomes.Add(new LootDistributionData.BiomeData { biome = biome, count = 10, probability = 0.05f });
		}
		LootDistributionHandler.AddLootDistributionData(creature.ClassID, creature.PrefabInfo.PrefabFileName, biomes.ToArray());

		ItemActionHandler.RegisterMiddleClickAction(creature.PrefabInfo.TechType, _ => InventorySounds.Play(), "pull ahoge", "English");
    }

    private static void LoadErmfishVariant(PrefabInfo info, string iconPath, RecipeData recipe, float foodValue, float waterValue, bool decomposes, string survivalCraftingTab, TechCategory techCategory, int childModelIndex)
    {
	    CustomPrefab variant = new(info);
		variant.Info.WithIcon(AssetLoader.GetAtlasSprite(iconPath));

		CraftingGadget crafting = new(variant, recipe);
		crafting.WithFabricatorType(CraftTree.Type.Fabricator);
		crafting.WithStepsToFabricatorTab("Survival", survivalCraftingTab);
		variant.AddGadget(crafting);

		variant.SetGameObject(new CloneTemplate(variant.Info, ErmfishTypes.Regular.TechType)
		{
			ModifyPrefab = prefab =>
			{
				Eatable eatable = prefab.EnsureComponent<Eatable>();
				eatable.foodValue = foodValue;
				eatable.waterValue = waterValue;
				eatable.kDecayRate = 0.015f;
				eatable.decomposes = decomposes;

				// WM
				prefab.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
				prefab.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(childModelIndex).gameObject.SetActive(true);

				// VM
				prefab.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
				prefab.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(childModelIndex).gameObject.SetActive(true);
			}
		});
		variant.SetPdaGroupCategory(TechGroup.Survival, techCategory);
		variant.Register();

		CraftDataHandler.SetEquipmentType(variant.Info.TechType, EquipmentType.Hand);
		ItemActionHandler.RegisterMiddleClickAction(variant.Info.TechType, _ => InventorySounds.Play(), "pull ahoge", "English");
    }
}
