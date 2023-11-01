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
