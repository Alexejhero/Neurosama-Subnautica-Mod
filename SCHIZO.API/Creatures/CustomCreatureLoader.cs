using System;
using System.Collections.Generic;
using System.Linq;
using ECCLibrary;
using Nautilus.Handlers;
using SCHIZO.API.Sounds;
using SCHIZO.API.Unity.Creatures;

namespace SCHIZO.API.Creatures;

public abstract class CustomCreatureLoader<TData, TPrefab, TLoader> : ICustomCreatureLoader
    where TData : CustomCreatureData where TPrefab : CreatureAsset, ICustomCreaturePrefab where TLoader : CustomCreatureLoader<TData, TPrefab, TLoader>, new()
{
    public static TLoader Instance;

    protected readonly TData creatureData;
    protected readonly CreatureSounds creatureSounds;

    protected TPrefab prefab { get; private set; }

    protected string PDAEncyPath { get; init; }
    protected float ScanTime { get; init; } = 5;

    protected CustomCreatureLoader(TData data)
    {
        if (Instance != null) throw new Exception($"Only one instance of {typeof(TLoader).Name} can exist at a time.");

        Instance = (TLoader) this;
        creatureData = data;
        creatureSounds = new CreatureSounds(creatureData.soundData);
    }

    protected abstract TPrefab CreatePrefab();

    protected virtual void PostRegister()
    {
    }

    protected virtual IEnumerable<LootDistributionData.BiomeData> GetLootDistributionData()
    {
        yield break;
    }

    public virtual void Register()
    {
        prefab = CreatePrefab();
        ((ICustomCreaturePrefab) prefab).Register();

        CreatureDataUtils.AddCreaturePDAEncyclopediaEntry(prefab, PDAEncyPath, prefab.ModItem.DisplayName, creatureData.databankText.text, ScanTime, creatureData.databankTexture, creatureData.unlockSprite);

        CreatureSoundsHandler.RegisterCreatureSounds(prefab.ModItem, creatureSounds);

        LootDistributionData.BiomeData[] biomes = GetLootDistributionData().ToArray();
        if (biomes.Length > 0) LootDistributionHandler.AddLootDistributionData(prefab.PrefabInfo.ClassID, biomes);

        PostRegister();
    }
}

public interface ICustomCreatureLoader
{
    void Register();
}
