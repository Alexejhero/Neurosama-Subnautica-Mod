using System;
using System.Collections.Generic;
using System.Linq;
using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermfish;

public static class ErmfishLoader
{
    public static readonly SoundCollection2D CraftSounds = SoundCollection2D.Create("ermfish/cooking", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection3D DropSounds = SoundCollection3D.Create("ermfish/release", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection2D EatSounds = SoundCollection2D.Create("ermfish/eating", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection2D EquipSounds = SoundCollection2D.Create("ermfish/equipping", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection3D HurtSounds = SoundCollection3D.Create("ermfish/hurt", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection2D InventorySounds = SoundCollection2D.Create("ermfish/noises", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection2D PickupSounds = SoundCollection2D.Create("ermfish/pickup", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection2D PlayerDeathSounds = SoundCollection2D.Create("ermfish/player_death", "bus:/master/SFX_for_pause/nofilter");
    public static readonly SoundCollection2D ScanSounds = SoundCollection2D.Create("ermfish/scan", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection2D UnequipSounds = SoundCollection2D.Create("ermfish/unequipping", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection3D WorldSounds = SoundCollection3D.Create("ermfish/noises", AudioUtils.BusPaths.UnderwaterCreatures); // todo: get rid of fmod erros for loading the same audio file twice

    public static void Load()
    {
        LoadErmfish();
        LoadErmfishVariant(ModItems.CookedErmfish, "erm_cooked.png", new RecipeData(new CraftData.Ingredient(ModItems.Ermfish)), 23, 4, true, CraftTreeHandler.Paths.FabricatorCookedFood, TechCategory.CookedFood, 2);
        LoadErmfishVariant(ModItems.CuredErmfish, "erm_cured.png", new RecipeData(new CraftData.Ingredient(ModItems.Ermfish), new CraftData.Ingredient(TechType.Salt)), 23, -2, false, CraftTreeHandler.Paths.FabricatorCuredFood, TechCategory.CuredFood, 1);
    }

    private static void LoadErmfish()
    {
		ErmfishPrefab ermfish = new(ModItems.Ermfish);
		ermfish.PrefabInfo.WithIcon(AssetLoader.GetAtlasSprite("erm.png"));
		ermfish.Register();

		Texture2D databankTexture = AssetLoader.GetTexture("ermfish-databank.png");
        Sprite unlockSprite = AssetLoader.GetUnitySprite("ermfish-unlock.png");

        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(ermfish, "Lifeforms/Fauna/SmallHerbivores", "Ermfish",
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

			<size=75%>(Databank art by CJMAXiK)</size>
			""", 5, databankTexture, unlockSprite);

		List<LootDistributionData.BiomeData> biomes = new();
		foreach (BiomeType biome in Enum.GetValues(typeof(BiomeType)).Cast<BiomeType>())
		{
			biomes.Add(new LootDistributionData.BiomeData { biome = biome, count = 1, probability = 0.025f });
			biomes.Add(new LootDistributionData.BiomeData { biome = biome, count = 5, probability = 0.010f });
		}
		LootDistributionHandler.AddLootDistributionData(ermfish.ClassID, biomes.ToArray());

		ItemActionHandler.RegisterMiddleClickAction(ermfish.PrefabInfo.TechType, _ => InventorySounds.Play(), "pull ahoge", "English");
    }

    private static void LoadErmfishVariant(PrefabInfo info, string iconPath, RecipeData recipe, float foodValue, float waterValue, bool decomposes, string[] craftingTabPath, TechCategory techCategory, int childModelIndex)
    {
	    CustomPrefab variant = new(info);
		variant.Info.WithIcon(AssetLoader.GetAtlasSprite(iconPath));

		CraftingGadget crafting = new(variant, recipe);
		crafting.WithFabricatorType(CraftTree.Type.Fabricator);
		crafting.WithStepsToFabricatorTab(craftingTabPath);
		variant.AddGadget(crafting);

		variant.SetGameObject(new CloneTemplate(variant.Info, ModItems.Ermfish)
		{
			ModifyPrefab = prefab =>
			{
				Eatable eatable = prefab.EnsureComponent<Eatable>();
				eatable.foodValue = foodValue;
				eatable.waterValue = waterValue;
				eatable.kDecayRate = 0.015f;
				eatable.decomposes = decomposes;

				prefab.transform.Find("WM/erm/regular").gameObject.SetActive(false);
				prefab.transform.Find("WM/erm").GetChild(childModelIndex).gameObject.SetActive(true);

				prefab.transform.Find("VM/erm/regular").gameObject.SetActive(false);
				prefab.transform.Find("VM/erm").GetChild(childModelIndex).gameObject.SetActive(true);

                CreaturePrefabUtils.AddVFXFabricating(prefab, new VFXFabricatingData("VM/erm", -0.255f, 0.67275f, new Vector3(0, 0.22425f), 0.1f, new Vector3(0, -180, 0)));
			}
		});
		variant.SetPdaGroupCategory(TechGroup.Survival, techCategory);
		variant.Register();

		CraftDataHandler.SetEquipmentType(variant.Info.TechType, EquipmentType.Hand);
		ItemActionHandler.RegisterMiddleClickAction(variant.Info.TechType, _ => InventorySounds.Play(), "pull ahoge", "English");
    }

    public static List<TechType> ErmfishTechTypes => new() { ModItems.Ermfish, ModItems.CookedErmfish, ModItems.CuredErmfish };
}
