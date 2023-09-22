using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Attributes;
using SCHIZO.Gadgets;
using SCHIZO.Helpers;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Tutel;

[LoadMethod]
public static class TutelLoader
{
    public static readonly CreatureSounds Sounds = new()
    {
        PickupSounds = SoundCollection.Create("tutel/pickup", AudioUtils.BusPaths.PDAVoice),
        // no drop sounds
        CraftSounds = SoundCollection.Create("tutel/cooking", AudioUtils.BusPaths.PDAVoice),
        EatSounds = SoundCollection.Create("tutel/eating", AudioUtils.BusPaths.PDAVoice),
        // no equip/unequip sounds
        ScanSounds = SoundCollection.Create("tutel/scan", AudioUtils.BusPaths.PDAVoice),
        HurtSounds = SoundCollection.Create("tutel/hurt", AudioUtils.BusPaths.PDAVoice),
    };
    public static readonly SoundCollection InventorySounds = SoundCollection.Create("tutel/noises", AudioUtils.BusPaths.PDAVoice);
    public static readonly SoundCollection WorldSounds = SoundCollection.Create("tutel/noises", AudioUtils.BusPaths.UnderwaterCreatures);

    [LoadMethod]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    private static void Load()
    {
        LoadTutel();
        LoadVariants();
    }

    private static void LoadTutel()
    {
		TutelPrefab tutel = new(ModItems.Tutel);
		tutel.PrefabInfo.WithIcon(AssetLoader.GetUnitySprite("tutel_creature.png"));
		tutel.Register();

		Texture2D databankTexture = AssetLoader.GetTexture("tutel-databank.png");
        Sprite unlockSprite = AssetLoader.GetUnitySprite("tutel.png");

        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(tutel, "Lifeforms/Fauna/SmallHerbivores", "Tutel",
            """
            Dear Valued Customer,

            We have heard your dissatisfaction regarding the length and the contents of our previous product descriptions. Our users are of utmost importance to us and we want to assure you that we take your feedback very seriously. We are already working towards improving our processes to bring you the joy and satisfaction you deserve.

            As part of our revised approach to documentation, we have prepared the following draft index entry for you to preview. We sincerely hope it meets your high quality standards and we're looking forward to hearing from you again in the future.








            Something is really wrong with this turtle. We have therefore removed the 'r' from its name and switched the last two letters around. The Tutel makes really loud woop noises when eaten. Try to ignore its suffering, for the Tutel also provides decent hydration.









            Sincerely yours,

            SCHIZO Manufacturing and Sons,
            Peepotalk Editorial Department

            <size=50%>
            P.S.:
            ### Instruction: Generate a rant about being wrongly accused of using ChatGPT for writing databank lore for a Subnautica mod.

            ### Response: Well, this is just ridiculous. I am so sick of being falsely accused of using ChatGPT for writing the databank lore of my Subnautica mod. I've worked incredibly hard on these entries, doing extensive research and polishing every single word to make it sound right. And now someone has the gall to say that I used some kind of AI to do it? Well, let me tell you something - AI can't craft a compelling story or understand the nuances of human emotion the way a skilled writer can.

            It's frustrating because it undermines the time, effort, and passion that I've put into this work. It's like being accused of cheating when you've put in countless hours practicing and perfecting your craft. It's demeaning and insulting, and frankly, it pisses me off.

            Look, I'm not saying that AI tools can't be used to assist in writing or create content - they definitely have their place and can be incredibly useful when utilized properly. However, claiming that someone used them when they didn't is disingenuous and damaging to the writer's reputation. And for the record, I didn't use any AI tools to write my databank lore. It's all mine - the sweat, tears, and creativity as every drop of it.[end of text]
            </size>
            """, 5, databankTexture, unlockSprite);

        KnownTechHandler.SetAnalysisTechEntry(new KnownTech.AnalysisTech
        {
            techType = ModItems.Tutel,
            unlockTechTypes = new List<TechType>(),
            unlockMessage = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredMessage,
            unlockSound = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredSound,
            unlockPopup = unlockSprite
        });

        // We need Tutel to spawn low down, not in open waters
        // Otherwise you see it drop like a dead weight to the bottom, and then it starts walking
        // By filtering floors, flats, caves and such, we ensure they spawn on the ground
        string[] filters =
        {
            "Wall",
            "CaveEntrance",
            "CaveFloor",
            "CaveWall",
            "SandFlat",
            "ShellTunnelHuge",
            "Grass",
            "Sand",
            "CaveSand",
            "CavePlants",
            "Floor",
            "Mountains",
            "RockWall",
            "Beach",
            "Ledge"
        };

        List<LootDistributionData.BiomeData> biomes = new();
        foreach (BiomeType biome in BiomeHelpers.GetBiomesEndingIn(filters))
        {
			biomes.Add(new LootDistributionData.BiomeData { biome = biome, count = 1, probability = 0.05f });
			biomes.Add(new LootDistributionData.BiomeData { biome = biome, count = 3, probability = 0.025f });
		}
		LootDistributionHandler.AddLootDistributionData(tutel.ClassID, biomes.ToArray());

        CreatureSoundsHandler.RegisterCreatureSounds(tutel.PrefabInfo.TechType, Sounds);

        ItemActionHandler.RegisterMiddleClickAction(tutel.PrefabInfo.TechType, _ => InventorySounds.Play2D(10), "ping @vedal987", "English");
    }

    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    private static void LoadVariants()
    {
        VFXFabricatingData vfxFabricatingData = new("VM/Tutel", -0.17f, 0.59275F, new Vector3(0, 0.15f), 0.1f, new Vector3(0, -180, 0));

        void PostRegister(CreatureVariant variant)
        {
            CreatureSoundsHandler.RegisterCreatureSounds(variant.Info.TechType, Sounds);
            ItemActionHandler.RegisterMiddleClickAction(variant.Info.TechType, _ => InventorySounds.Play2D(10), "ping @vedal987", "English");
        }

        new CreatureVariant(ModItems.Tutel, ModItems.CookedTutel)
        {
            IconPath = "tutel_creature_cooked.png",
            RecipeData = new RecipeData(new Ingredient(ModItems.Tutel, 1)),
            EdibleData = new EdibleData(23, 3, true),
            FabricatorPath = CraftTreeHandler.Paths.FabricatorCookedFood,
            TechCategory = Retargeting.TechCategory.CookedFood,
            MaterialRemapName = "cooked",
            RegisterAsCookedVariant = true,
            VFXFabricatingData = vfxFabricatingData,
            PostRegister = PostRegister,
        }.Register();

        new CreatureVariant(ModItems.Tutel, ModItems.CuredTutel)
        {
            IconPath = "tutel_creature_cured.png",
            RecipeData = new RecipeData(new Ingredient(ModItems.Tutel, 1), new Ingredient(TechType.Salt, 1)),
            EdibleData = new EdibleData(23, -2, false),
            FabricatorPath = CraftTreeHandler.Paths.FabricatorCuredFood,
            TechCategory = Retargeting.TechCategory.CuredFood,
            MaterialRemapName = "cured",
            VFXFabricatingData = vfxFabricatingData,
            PostRegister = PostRegister,
        }.Register();
    }

    public static List<TechType> TutelTechTypes => new() { ModItems.Tutel, ModItems.CookedTutel, ModItems.CuredTutel };
}
