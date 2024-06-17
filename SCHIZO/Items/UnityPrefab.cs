using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nautilus.Assets;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Helpers;
using SCHIZO.Items.Data;
using SCHIZO.Registering;
using SCHIZO.Spawns;
using UnityEngine;
using UWE;
using Object = UnityEngine.Object;

namespace SCHIZO.Items;

public class UnityPrefab : CustomPrefab
{
    #region Prefab cache

    private static readonly Transform _prefabCacheParent;

    static UnityPrefab()
    {
        _prefabCacheParent = new GameObject("SCHIZO Prefab Cache").transform;
        _prefabCacheParent.gameObject.SetActive(false);
        Object.DontDestroyOnLoad(_prefabCacheParent);
    }

    #endregion

    internal ModItem ModItem { get; }
    protected ItemData UnityData => ModItem.ItemData;
    protected PrefabInfo PrefabInfo => ModItem.PrefabInfo;

    [SetsRequiredMembers]
    // ReSharper disable once ConvertToPrimaryConstructor
    public UnityPrefab(ModItem item) : base(item)
    {
        ModItem = item;
    }

    public new void Register()
    {
        NautilusPrefabConvertible prefab = GetPrefab();
        if (prefab != null) this.SetGameObject(prefab);

        base.Register();
        SetItemProperties();
    }

    protected virtual NautilusPrefabConvertible GetPrefab()
    {
        if (!UnityData.prefab) return null;

        GameObject instance = Object.Instantiate(UnityData.prefab, _prefabCacheParent);

        SetupComponents(instance);
        ModifyPrefab(instance);

        return instance;
    }

    protected virtual void ModifyPrefab(GameObject prefab)
    {
        MaterialUtils.ApplySNShaders(prefab, 1);
        prefab.GetComponents<IPrefabInit>().ForEach(iPI => iPI.PrefabInit(prefab));
    }

    protected virtual void SetItemProperties()
    {
        if (ModItem.ItemData.isCraftable && ModItem.ItemData.CraftTreeType != CraftTree.Type.None)
        {
            CraftTreeHandler.AddCraftingNode(ModItem.ItemData.CraftTreeType, ModItem, ModItem.ItemData.CraftTreePath);
            CraftDataHandler.SetCraftingTime(ModItem, ModItem.ItemData.craftingTime);
        }

        if (ModItem.ItemData.isBuildable)
        {
            CraftDataHandler.AddBuildable(ModItem);
        }

        if (ModItem.ItemData.Recipe)
        {
            CraftDataHandler.SetRecipeData(ModItem, ModItem.ItemData.Recipe.Convert());
        }

        if (ModItem.ItemData.TechGroup != TechGroup.Uncategorized)
        {
            CraftDataHandler.AddToGroup(ModItem.ItemData.TechGroup, ModItem.ItemData.TechCategory, ModItem);
        }

        ModItem.ItemData.pdaEncyInfo!?.Register(this);
        ModItem.ItemData.knownTechInfo!?.Register(this);

        if (ModItem.ItemData.unlockAtStart)
        {
            KnownTechHandler.UnlockOnStart(ModItem);
        }
        else if (ModItem.ItemData.RequiredForUnlock != TechType.None)
        {
            KnownTechHandler.SetAnalysisTechEntry(new KnownTech.AnalysisTech
            {
                techType = ModItem.ItemData.RequiredForUnlock,
                unlockTechTypes = [ModItem],
            });
        }

        if (ModItem.ItemData.EquipmentType != EquipmentType.None)
        {
            CraftDataHandler.SetEquipmentType(ModItem, ModItem.ItemData.EquipmentType);
        }

        if (ModItem.ItemData.EquipmentType == EquipmentType.Hand)
        {
            if (ModItem.ItemData.QuickSlotType != QuickSlotType.None)
            {
                CraftDataHandler.SetQuickSlotType(ModItem, ModItem.ItemData.QuickSlotType);
            }

#if BELOWZERO
            if (ModItem.ItemData.coldResistanceBZ > 0)
            {
                CraftDataHandler.SetColdResistance(ModItem, ModItem.ItemData.coldResistanceBZ);
            }
#endif
        }

        if (ModItem.ItemData.spawnData && ModItem.ItemData.spawnData.Spawn)
        {
            List<LootDistributionData.BiomeData> lootDistData = [];

            foreach (BiomeType biome in ModItem.ItemData.spawnData.GetBiomes())
            {
                lootDistData.AddRange(ModItem.ItemData.spawnData.rules.Select(rule => rule.GetBiomeData(biome)));
            }

            if (lootDistData.Count > 0)
            {
                LootDistributionHandler.AddLootDistributionData(ModItem.PrefabInfo.ClassID, lootDistData.ToArray());

                LargeWorldEntity lwe = UnityData.prefab.GetComponentInChildren<LargeWorldEntity>();
                EntityTag entTag = UnityData.prefab.GetComponentInChildren<EntityTag>();
                if (!lwe) throw new InvalidOperationException($"{nameof(LargeWorldEntity)} missing on prefab {ModItem.PrefabInfo.ClassID}");
                if (!entTag) throw new InvalidOperationException($"{nameof(EntityTag)} missing on prefab {ModItem.PrefabInfo.ClassID}");
                // Required for LootDistribution/spawning system
                WorldEntityDatabaseHandler.AddCustomInfo(UnityData.classId, new WorldEntityInfo
                {
                    classId = UnityData.classId,
                    techType = ModItem,
                    cellLevel = lwe.cellLevel,
                    slotType = entTag.slotType,
                    localScale = Vector3.one,
                    prefabZUp = false,
                });
            }
        }

#if BELOWZERO
        if (!ModItem.ItemData.canBeRecycledBZ)
        {
            Recyclotron.bannedTech.Add(ModItem);
        }

        if (ModItem.ItemData.SoundTypeBZ != TechData.SoundType.Default)
        {
            CraftDataHandler.SetSoundType(ModItem, ModItem.ItemData.SoundTypeBZ);
        }
#endif
    }

    protected virtual void SetupComponents(GameObject instance)
    {
        instance.EnsureComponent<PrefabIdentifier>().classId = UnityData.classId;
        instance.EnsureComponent<TechTag>().type = ModItem;

        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
        if (renderers is {Length: > 0}) instance.EnsureComponent<SkyApplier>().renderers = renderers;

        Constructable constructable = instance.GetComponent<Constructable>();
        if (constructable)
        {
            constructable.techType = ModItem;
            constructable.ghostMaterial = MaterialHelpers.GhostMaterial;
#if BELOWZERO
            constructable._EmissiveTex = MaterialHelpers.ConstructableEmissiveTexture;
            constructable._NoiseTex = MaterialHelpers.ConstructableNoiseTexture;
#endif
        }
    }
}
