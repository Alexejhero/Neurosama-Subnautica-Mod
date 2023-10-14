using System;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using SCHIZO.Helpers;
using SCHIZO.Unity.Items;
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
        if (modItem.ItemData is CloneItemData cloneItemData)
        {
            cloneItemData.loader.Load();
            return;
        }

        if (modItem.ItemData is Unity.Creatures.CreatureData)
        {
            UnityCreaturePrefab.CreateAndRegister(modItem);
            return;
        }

        new UnityPrefab(modItem).Register();
    }

    [SetsRequiredMembers]
    protected UnityPrefab(ModItem item) : base(item)
    {
        ModItem = item;
    }

    public new virtual void Register()
    {
        ModItem.LoadStep2();

        NautilusPrefabConvertible prefab = GetPrefab();
        if (prefab != null) this.SetGameObject(prefab);

        base.Register();
        PostRegister();
    }

    protected virtual NautilusPrefabConvertible GetPrefab()
    {
        if (!UnityData.prefab) return null;

        return (Func<GameObject>) getDeferred;

        GameObject getDeferred()
        {
            GameObject instance = Object.Instantiate(UnityData.prefab, _prefabCacheParent);

            AddBasicComponents(instance);
            InitializeConstructable(instance);

            ModifyPrefab(instance);

            return instance;
        }
    }

    protected virtual void ModifyPrefab(GameObject prefab)
    {
        MaterialHelpers.ApplySNShadersIncludingRemaps(prefab, 1);
    }

    protected virtual void PostRegister()
    {
    }

    private void AddBasicComponents(GameObject instance)
    {
        instance.EnsureComponent<PrefabIdentifier>().classId = UnityData.classId;
        instance.EnsureComponent<TechTag>().type = ModItem;

        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
        if (renderers is {Length: > 0}) instance.EnsureComponent<SkyApplier>().renderers = renderers;
    }

    private void InitializeConstructable(GameObject instance)
    {
        Constructable constructable = instance.GetComponent<Constructable>();

        constructable.techType = ModItem;
        constructable.ghostMaterial = MaterialHelpers.GhostMaterial;
#if BELOWZERO
        constructable._EmissiveTex = MaterialHelpers.ConstructableEmissiveTexture;
        constructable._NoiseTex = MaterialHelpers.ConstructableNoiseTexture;
#endif
    }
}
