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

    public static void CreateAndRegister(ModItem modItem)
    {
#if SUBNAUTICA
        if (!modItem.ItemData.registerInSN)
        {
            LOGGER.LogMessage($"Not registering {modItem.ItemData.classId} in SN");
            return;
        }
#else
        if (!modItem.ItemData.registerInBZ)
        {
            LOGGER.LogMessage($"Not registering {modItem.ItemData.classId} in BZ");
            return;
        }
#endif

        if (modItem.ItemData is CloneItemData cloneItemData)
        {
            LOGGER.LogDebug($"Creating prefab {cloneItemData.loader.GetType().Name} for {modItem.ItemData.classId}");
            cloneItemData.loader.Load();
            return;
        }

        if (modItem.ItemData is Creatures.CreatureData)
        {
            LOGGER.LogDebug($"Creating prefab {nameof(UnityCreaturePrefab)} for {modItem.ItemData.classId}");
            new UnityCreaturePrefab(modItem).Register();
            return;
        }

        LOGGER.LogDebug($"Creating prefab {nameof(UnityPrefab)} for {modItem.ItemData.classId}");
        new UnityPrefab(modItem).Register();
    }

    [SetsRequiredMembers]
    protected UnityPrefab(ModItem item) : base(item)
    {
        ModItem = item;
    }

    public new virtual void Register()
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

#if BELOWZERO
        if (!ModItem.ItemData.canBeRecycledBZ)
        {
            Recyclotron.bannedTech.Add(ModItem.ItemData.ModItem);
        }

        if (ModItem.ItemData.SoundType != TechData.SoundType.Default)
        {
            CraftDataHandler.SetSoundType(ModItem, ModItem.ItemData.SoundType);
        }
#endif

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

        if (ModItem.ItemData.UnlockAtStart)
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
