using System.Collections.Generic;
using System.Linq;
using ECCLibrary;
using Nautilus.Handlers;
using SCHIZO.Sounds;
using SCHIZO.Unity.Creatures;

namespace SCHIZO.Creatures;

public abstract class CustomCreatureLoader<TData, TPrefab, TLoader> : ICustomCreatureLoader
    where TData : CustomCreatureData where TPrefab : CustomCreaturePrefab where TLoader : CustomCreatureLoader<TData, TPrefab, TLoader>, new()
{
    protected readonly TData creatureData;
    protected readonly CreatureSounds creatureSounds;

    protected TPrefab prefab { get; private set; }

    protected string PDAEncyPath { get; init; }
    protected float ScanTime { get; init; } = 5;

    protected CustomCreatureLoader(TData data)
    {
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
        prefab.Register();

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
