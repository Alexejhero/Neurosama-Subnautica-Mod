using System;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using Nautilus.Utility;
using SCHIZO.Helpers;
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

    protected readonly ModItem modItem;

    public static void CreateAndRegister(ModItem modItem)
    {
        new UnityPrefab(modItem).Register();
    }

    [SetsRequiredMembers]
    public UnityPrefab(ModItem item) : base(item)
    {
        modItem = item;
    }

    public new void Register()
    {
        modItem.LoadStep2();

        NautilusPrefabConvertible prefab = GetPrefab();
        if (prefab != null) this.SetGameObject(prefab);

        base.Register();
        PostRegister();
    }

    protected virtual NautilusPrefabConvertible GetPrefab()
    {
        if (!modItem.ItemData.prefab) return null;

        return (Func<GameObject>) getDeferred;

        GameObject getDeferred()
        {
            GameObject instance = Object.Instantiate(modItem.ItemData.prefab, _prefabCacheParent);

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
        instance.EnsureComponent<PrefabIdentifier>().classId = modItem.ItemData.classId;
        instance.EnsureComponent<TechTag>().type = modItem;

        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
        if (renderers is {Length: > 0}) instance.EnsureComponent<SkyApplier>().renderers = renderers;
    }

    private void InitializeConstructable(GameObject instance)
    {
        Constructable constructable = instance.GetComponent<Constructable>();

        constructable.techType = modItem;
        constructable.ghostMaterial = MaterialUtils.GhostMaterial;

        // TODO: more stuff for bz
        // constructable._tex
    }
}
