using System.Diagnostics.CodeAnalysis;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace SCHIZO.Items.Gymbag;

public sealed class Gymbag : ClonePrefab
{
    [SetsRequiredMembers]
    public Gymbag(ModItem modItem, TechType original) : base(modItem, original)
    {
    }

    protected override void ModifyClone(GameObject prefab)
    {
        StorageContainer container = prefab.GetComponentInChildren<StorageContainer>();
        container.width = 4;
        container.height = 4;

        GameObject baseModel = prefab.GetComponentInChildren<Renderer>().gameObject;
        baseModel.SetActive(false);

        GameObject instance = Object.Instantiate(modItem.ItemData.prefab, baseModel.transform.parent);

        PrefabUtils.AddVFXFabricating(instance, null, 0, 0.93f, new Vector3(0, -0.05f), 0.75f, Vector3.zero);
    }

    protected override void PostRegister()
    {
        ItemActionHandler.RegisterMiddleClickAction(Info.TechType, item => GymbagBehaviour.Instance.OnOpen(item), "open storage", "English");
    }
}
