using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using Nautilus.Handlers;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Items;

public abstract class BaseCustomPrefab : CustomPrefab
{
    #region Keep alive

    //todo: see if this is actually needed

    private static readonly Transform _keepAliveParent;

    static BaseCustomPrefab()
    {
        _keepAliveParent = new GameObject("KeepAlive").transform;
        _keepAliveParent.gameObject.SetActive(false);
        Object.DontDestroyOnLoad(_keepAliveParent);
    }

    #endregion

    protected readonly ModItem modItem;

    [SetsRequiredMembers]
    public BaseCustomPrefab(ModItem item) : base(item)
    {
        modItem = item;
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
        GameObject instance = Object.Instantiate(modItem.ItemData.prefab, _keepAliveParent);

        instance.EnsureComponent<PrefabIdentifier>().classId = modItem.ItemData.classId;
        instance.AddComponent<TechTag>().type = modItem;

        Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
        if (renderers is {Length: > 0}) instance.EnsureComponent<SkyApplier>().renderers = renderers;

        // ModifyPrefab(pre);

        MaterialHelpers.ApplySNShadersIncludingRemaps(instance, 1);

        return instance;
    }
}
