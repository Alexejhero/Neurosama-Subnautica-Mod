using System;
using System.Collections.Generic;
using System.Linq;
using ECCLibrary;
using Nautilus.Handlers;
using SCHIZO.Sounds;
using SCHIZO.Unity.Creatures;
using UnityEngine;

namespace SCHIZO.Creatures;

public abstract class CustomCreatureLoader<TData, TPrefab, TLoader> : ICustomCreatureLoader
    where TData : CustomCreatureData
    where TPrefab : CreatureAsset, IItemRegisterer
    where TLoader : CustomCreatureLoader<TData, TPrefab, TLoader>, new()
{
    public static TLoader Instance;

    public readonly HashSet<TechType> TechTypes = new();

    protected readonly TData creatureData;
    protected readonly CreatureSounds creatureSounds;

    protected TPrefab regularPrefab { get; private set; }

    protected string PDAEncyPath { get; init; }
    protected float ScanTime { get; init; } = 5;

    protected CustomCreatureLoader(TData data)
    {
        if (Instance != null) throw new Exception($"Only one instance of {typeof(TLoader).Name} can exist at a time.");

        Instance = (TLoader) this;
        creatureData = data;
        creatureSounds = new CreatureSounds(creatureData.soundData);
    }

    protected abstract TPrefab CreatePrefab(GameObject rawObject);

    protected virtual void PostRegister(TPrefab prefab)
    {
    }

    protected virtual IEnumerable<LootDistributionData.BiomeData> GetLootDistributionData()
    {
        return Enumerable.Empty<LootDistributionData.BiomeData>();
    }

    public virtual void Register()
    {
        regularPrefab = RegisterPrefab(creatureData.regularPrefab);

        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(regularPrefab, PDAEncyPath, regularPrefab.ModItem.DisplayName, creatureData.databankText.text, ScanTime, creatureData.databankTexture, creatureData.unlockSprite);

        LootDistributionData.BiomeData[] biomes = GetLootDistributionData().ToArray();
        if (biomes.Length > 0) LootDistributionHandler.AddLootDistributionData(regularPrefab.PrefabInfo.ClassID, biomes);
    }

    private TPrefab RegisterPrefab(GameObject rawObject)
    {
        TPrefab creaturePrefab = CreatePrefab(rawObject);
        ((IItemRegisterer) creaturePrefab).Register();

        TechTypes.Add(creaturePrefab.ModItem);

        CreatureSoundsHandler.RegisterCreatureSounds(creaturePrefab.ModItem, creatureSounds);

        PostRegister(creaturePrefab);

        return creaturePrefab;
    }
}

public interface ICustomCreatureLoader
{
    void Register();
}
