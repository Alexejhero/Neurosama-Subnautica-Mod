using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using Nautilus.Utility;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Items;

public class UnityPrefab : CustomPrefab
{
    #region Keep alive

    //todo: see if this is actually needed

    private static readonly Transform _keepAliveParent;

    static UnityPrefab()
    {
        _keepAliveParent = new GameObject("KeepAlive").transform;
        _keepAliveParent.gameObject.SetActive(false);
        Object.DontDestroyOnLoad(_keepAliveParent);
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

    public new virtual void Register()
    {
        modItem.LoadStep2();
        SetGameObject(GetPrefab);
        base.Register();
    }

    protected virtual GameObject GetPrefab()
    {
        GameObject instance = Object.Instantiate(modItem.ItemData.prefab, _keepAliveParent);

        AddBasicComponents(instance);
        InitializeConstructable(instance);

        // ModifyPrefab(pre);

        MaterialHelpers.ApplySNShadersIncludingRemaps(instance, 1);

        return instance;
    }

    private void AddBasicComponents(GameObject instance)
    {
        instance.EnsureComponent<PrefabIdentifier>().classId = modItem.ItemData.classId;
        instance.AddComponent<TechTag>().type = modItem;

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
