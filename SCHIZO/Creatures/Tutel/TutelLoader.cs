using System.Collections.Generic;
using ECCLibrary.Data;
using Nautilus.Handlers;
using SCHIZO.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.Unity.Creatures;
using UnityEngine;

namespace SCHIZO.Creatures.Tutel;

[LoadCreature]
public sealed class TutelLoader : PickupableCreatureLoader<PickupableCreatureData, TutelPrefab, TutelLoader>
{
    public TutelLoader() : base(ResourceManager.LoadAsset<PickupableCreatureData>("Tutel data"))
    {
        PDAEncyPath = "Lifeforms/Fauna/SmallHerbivores";
        VariantsAreAlive = true;
        VFXFabricatingData = new VFXFabricatingData("VM/Tutel", -0.17f, 0.59275F, new Vector3(0, 0.15f), 0.1f, new Vector3(0, -180, 0));
    }

    protected override TutelPrefab CreatePrefab()
    {
        return new TutelPrefab(ModItems.Tutel, ModItems.CookedTutel, ModItems.CuredTutel, creatureData.prefab);
    }

    protected override IEnumerable<LootDistributionData.BiomeData> GetLootDistributionData()
    {
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

        foreach (BiomeType biome in BiomeHelpers.GetBiomesEndingIn(filters))
        {
            yield return new LootDistributionData.BiomeData { biome = biome, count = 1, probability = 0.05f };
            yield return new LootDistributionData.BiomeData { biome = biome, count = 3, probability = 0.025f };
        }
    }

    protected override void PostRegisterAlive(ModItem item)
    {
        base.PostRegisterAlive(item);
        ItemActionHandler.RegisterMiddleClickAction(item, _ => creatureSounds.AmbientItemSounds.Play2D(10), "ping @vedal987", "English");
    }
}
