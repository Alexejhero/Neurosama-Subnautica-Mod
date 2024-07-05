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
using UnityEngine;
using Object = UnityEngine.Object;

namespace SCHIZO.Items;

[method: SetsRequiredMembers]
public class UnityPrefab(ModItem item) : CustomPrefab(item)
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

    internal ModItem ModItem { get; } = item;
    protected ItemData UnityData => ModItem.ItemData;
    protected PrefabInfo PrefabInfo => ModItem.PrefabInfo;

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
        if (UnityData.isCraftable && UnityData.CraftTreeType != CraftTree.Type.None)
        {
            CraftTreeHandler.AddCraftingNode(UnityData.CraftTreeType, ModItem, UnityData.CraftTreePath);
            CraftDataHandler.SetCraftingTime(ModItem, UnityData.craftingTime);
        }

        if (UnityData.isBuildable)
        {
            CraftDataHandler.AddBuildable(ModItem);
        }

        if (UnityData.Recipe)
        {
            CraftDataHandler.SetRecipeData(ModItem, UnityData.Recipe.Convert());
        }

        if (UnityData.TechGroup != TechGroup.Uncategorized)
        {
            CraftDataHandler.AddToGroup(UnityData.TechGroup, UnityData.TechCategory, ModItem);
        }

        if (UnityData.pdaEncyInfo) UnityData.pdaEncyInfo.Register(this);
        if (UnityData.knownTechInfo) UnityData.knownTechInfo.Register(this);

        if (UnityData.unlockAtStart)
        {
            KnownTechHandler.UnlockOnStart(ModItem);
        }
        else if (UnityData.RequiredForUnlock != TechType.None)
        {
            KnownTechHandler.SetAnalysisTechEntry(new KnownTech.AnalysisTech
            {
                techType = UnityData.RequiredForUnlock,
                unlockTechTypes = [ModItem],
            });
        }

        if (UnityData.EquipmentType != EquipmentType.None)
        {
            CraftDataHandler.SetEquipmentType(ModItem, UnityData.EquipmentType);
        }

        if (UnityData.EquipmentType == EquipmentType.Hand)
        {
            if (UnityData.QuickSlotType != QuickSlotType.None)
            {
                CraftDataHandler.SetQuickSlotType(ModItem, UnityData.QuickSlotType);
            }

#if BELOWZERO
            if (UnityData.coldResistanceBZ > 0)
            {
                CraftDataHandler.SetColdResistance(ModItem, UnityData.coldResistanceBZ);
            }
#endif
        }

        if (UnityData.spawnData && UnityData.spawnData.Spawn)
        {
            List<LootDistributionData.BiomeData> lootDistData = [];

            foreach (BiomeType biome in UnityData.spawnData.GetBiomes())
            {
                lootDistData.AddRange(UnityData.spawnData.rules.Select(rule => rule.GetBiomeData(biome)));
            }

            if (lootDistData.Count > 0)
            {
                LootDistributionHandler.AddLootDistributionData(PrefabInfo.ClassID, [.. lootDistData]);

                LargeWorldEntity lwe = UnityData.prefab.GetComponentInChildren<LargeWorldEntity>();
                EntityTag entTag = UnityData.prefab.GetComponentInChildren<EntityTag>();
                if (!lwe) throw new InvalidOperationException($"{nameof(LargeWorldEntity)} missing on prefab {PrefabInfo.ClassID}");
                if (!entTag) throw new InvalidOperationException($"{nameof(EntityTag)} missing on prefab {PrefabInfo.ClassID}");
                // Required for LootDistribution/spawning system
                WorldEntityDatabaseHandler.AddCustomInfo(UnityData.classId, new()
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
        if (!UnityData.canBeRecycledBZ)
        {
            Recyclotron.bannedTech.Add(ModItem);
        }

        if (UnityData.SoundTypeBZ != TechData.SoundType.Default)
        {
            CraftDataHandler.SetSoundType(ModItem, UnityData.SoundTypeBZ);
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
