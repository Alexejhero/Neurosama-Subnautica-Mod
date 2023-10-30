using System.Diagnostics.CodeAnalysis;
using Nautilus.Handlers;
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
        container.storageLabel = IS_BELOWZERO ? "Quantum Gymbag" : "Gymbag";

        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        renderers.ForEach(r => r.gameObject.SetActive(false));

        GameObject.Destroy(prefab.GetComponentInChildren<VFXFabricating>());

        Object.Instantiate(UnityData.prefab, renderers[0].transform.parent);
    }

    protected override void PostRegister()
    {
        ItemActionHandler.RegisterMiddleClickAction(Info.TechType, item => GymbagManager.Instance.OnOpen(item), "open storage", "English");
    }
}
