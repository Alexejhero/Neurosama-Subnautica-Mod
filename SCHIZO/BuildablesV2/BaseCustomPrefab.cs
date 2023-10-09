using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using SCHIZO.Helpers;
using SCHIZO.Unity.Items;
using UnityEngine;

namespace SCHIZO.BuildablesV2;

public abstract class BaseCustomPrefab : CustomPrefab
{
    private static readonly Transform _keepAliveParent;

    static BaseCustomPrefab()
    {
        _keepAliveParent = new GameObject("KeepAlive").transform;
        _keepAliveParent.gameObject.SetActive(false);
        Object.DontDestroyOnLoad(_keepAliveParent);
    }

    protected readonly ModItem modItem;
    protected readonly ItemData itemData;

    [SetsRequiredMembers]
    public BaseCustomPrefab(ModItem item, ItemData data) : base(item)
    {
        modItem = item;
        itemData = data;
    }

    public new virtual void Register()
    {
        //AddBasicGadgets();
        //AddGadgets();

        SetGameObject(GetPrefab);

        base.Register();
        //PostRegister();
    }

    protected virtual GameObject GetPrefab()
    {
        GameObject instance = Object.Instantiate(itemData.prefab, _keepAliveParent);

        instance.EnsureComponent<PrefabIdentifier>().classId = itemData.classId;
        instance.AddComponent<TechTag>().type = modItem;

        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
        if (renderers is {Length: > 0}) instance.EnsureComponent<SkyApplier>().renderers = renderers;

        // ModifyPrefab(pre);

        MaterialHelpers.ApplySNShadersIncludingRemaps(instance, 1);

        return instance;
    }
}
