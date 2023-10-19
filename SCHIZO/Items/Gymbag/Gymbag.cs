using System.Diagnostics.CodeAnalysis;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace SCHIZO.Items.Gymbag;

public sealed class Gymbag : ClonePrefab
{
    [SetsRequiredMembers]
    public Gymbag(ModItem modItem, TechType cloned) : base(modItem, cloned)
    {
    }

    protected override void ModifyClone(GameObject prefab)
    {
        StorageContainer container = prefab.GetComponentInChildren<StorageContainer>();
        container.width = 4;
        container.height = 4;

        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        renderers.ForEach(r => r.gameObject.SetActive(false));

        GameObject.Destroy(prefab.GetComponentInChildren<VFXFabricating>());

        GameObject instance = Object.Instantiate(UnityData.prefab, renderers[0].transform.parent);

        PrefabUtils.AddVFXFabricating(instance, null, 0, 0.93f, new Vector3(0, -0.05f), 0.75f, Vector3.zero);
    }

    protected override void PostRegister()
    {
        ItemActionHandler.RegisterMiddleClickAction(Info.TechType, item => GymbagManager.Instance.OnOpen(item), "open storage", "English");
    }
}
