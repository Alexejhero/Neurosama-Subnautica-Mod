using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using SCHIZO.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.Sounds;
using SCHIZO.Unity.Creatures;

namespace SCHIZO.Creatures.Tutel;

[LoadMethod]
public sealed class TutelLoader : PickupableCreatureLoader
{
    [LoadMethod]
    private static void Load()
    {
        PickupableCreatureData data = ResourceManager.LoadAsset<PickupableCreatureData>("Tutel data");

        TutelLoader loader = new(data);
        loader.LoadTutel();
        loader.LoadVariants();
    }

    private TutelLoader(PickupableCreatureData data) : base(data)
    {
    }

    private void LoadTutel()
    {
		TutelPrefab tutel = new(ModItems.Tutel, _creatureData.prefab);
		tutel.PrefabInfo.WithIcon(_creatureData.regularIcon);
		tutel.Register();

        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(tutel, "Lifeforms/Fauna/SmallHerbivores", "Tutel", _creatureData.databankText.text, 5, _creatureData.databankTexture, _creatureData.unlockSprite);

        KnownTechHandler.SetAnalysisTechEntry(new KnownTech.AnalysisTech
        {
            techType = ModItems.Tutel,
            unlockTechTypes = new List<TechType>(),
            unlockMessage = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredMessage,
            unlockSound = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredSound,
            unlockPopup = _creatureData.unlockSprite
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

        PostRegister(tutel.PrefabInfo);
    }

    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    private void LoadVariants()
    {
        VFXFabricatingData vfxFabricatingData = new("VM/Tutel", -0.17f, 0.59275F, new Vector3(0, 0.15f), 0.1f, new Vector3(0, -180, 0));

        new CreatureVariant(ModItems.Tutel, ModItems.CookedTutel)
        {
            Icon = _creatureData.cookedIcon,
            RecipeData = new RecipeData(new Ingredient(ModItems.Tutel, 1)),
            EdibleData = new EdibleData(23, 3, true),
            FabricatorPath = CraftTreeHandler.Paths.FabricatorCookedFood,
            TechCategory = Retargeting.TechCategory.CookedFood,
            MaterialRemap = _creatureData.cookedRemap,
            RegisterAsCookedVariant = true,
            VFXFabricatingData = vfxFabricatingData,
            PostRegister = PostRegister,
        }.Register();

        new CreatureVariant(ModItems.Tutel, ModItems.CuredTutel)
        {
            Icon = _creatureData.curedIcon,
            RecipeData = new RecipeData(new Ingredient(ModItems.Tutel, 1), new Ingredient(TechType.Salt, 1)),
            EdibleData = new EdibleData(23, -2, false),
            FabricatorPath = CraftTreeHandler.Paths.FabricatorCuredFood,
            TechCategory = Retargeting.TechCategory.CuredFood,
            MaterialRemap = _creatureData.curedRemap,
            VFXFabricatingData = vfxFabricatingData,
            PostRegister = PostRegister,
        }.Register();
    }

    private void PostRegister(PrefabInfo info)
    {
        CreatureSoundsHandler.RegisterCreatureSounds(info.TechType, _creatureSounds);
        ItemActionHandler.RegisterMiddleClickAction(info.TechType, _ => _creatureSounds.AmbientItemSounds.Play2D(10), "ping @vedal987", "English");
    }

    public static List<TechType> TutelTechTypes => new() { ModItems.Tutel, ModItems.CookedTutel, ModItems.CuredTutel };
}
