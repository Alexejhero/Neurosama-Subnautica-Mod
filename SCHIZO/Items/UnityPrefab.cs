using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Creatures;
using SCHIZO.Helpers;
using SCHIZO.Items.Data;
using SCHIZO.Sounds;
using UnityEngine;
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

    protected ModItem ModItem { get; }
    protected ItemData UnityData => ModItem.ItemData;
    protected PrefabInfo PrefabInfo => ModItem.PrefabInfo;

    [SetsRequiredMembers]
    public UnityPrefab(ModItem item) : base(item)
    // ReSharper disable once ConvertToPrimaryConstructor
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

        if (ModItem.ItemData.pdaEncyInfo)
        {
            PDAEncyclopediaInfo i = ModItem.ItemData.pdaEncyInfo;
            string encyPath = RetargetHelpers.Pick(i.encyPathSN, i.encyPathBZ);

            PDAHandler.AddEncyclopediaEntry(ModItem.PrefabInfo.ClassID, encyPath, i.title, i.description.text, i.texture, i.unlockSprite,
                i.isImportantUnlock ? PDAHandler.UnlockImportant : PDAHandler.UnlockBasic);

            if (i.scanSounds) ScanSoundHandler.Register(ModItem, i.scanSounds);
        }

        if (ModItem.ItemData.KnownTechInfo)
        {
            KnownTechInfo i = ModItem.ItemData.KnownTechInfo;

            KnownTechHandler.SetAnalysisTechEntry(new KnownTech.AnalysisTech
            {
                techType = ModItem,
                unlockTechTypes = new List<TechType>(0),
                unlockMessage = i.UnlockMessage,
                unlockSound = i.UnlockSound,
                unlockPopup = i.unlockSprite
            });
        }

        if (ModItem.ItemData.unlockAtStart)
        {
            KnownTechHandler.UnlockOnStart(ModItem);
        }
        else if (ModItem.ItemData.RequiredForUnlock != TechType.None)
        {
            KnownTechHandler.SetAnalysisTechEntry(new KnownTech.AnalysisTech
            {
                techType = ModItem.ItemData.RequiredForUnlock,
                unlockTechTypes = new List<TechType> {ModItem},
            });
        }

        if (ModItem.ItemData.itemSounds)
        {
            ModItem.ItemData.itemSounds.Register(ModItem);
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

#if BELOWZERO
        if (!ModItem.ItemData.canBeRecycledBZ)
        {
            Recyclotron.bannedTech.Add(ModItem.ItemData.ModItem);
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
