using System.Diagnostics.CodeAnalysis;
using Nautilus.Handlers;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Items.Gymbag;

public sealed class Gymbag : ClonePrefab
{
    private const TechType CLONE_TARGET =
#if SUBNAUTICA
        TechType.LuggageBag;
#else
        TechType.QuantumLocker;
#endif

    [SetsRequiredMembers]
    public Gymbag(ModItem modItem) : base(modItem, CLONE_TARGET)
    {
    }

    protected override void ModifyClone(GameObject prefab)
    {
        StorageContainer container = prefab.GetComponentInChildren<StorageContainer>();
        container.width = 4;
        container.height = 4;
        container.storageLabel = IS_BELOWZERO ? "Quantum Gymbag" : "Gymbag"; // todo: check if this works

        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        renderers.ForEach(r => r.gameObject.SetActive(false));

        GameObject.Destroy(prefab.GetComponentInChildren<VFXFabricating>());

        Object.Instantiate(UnityData.prefab, renderers[0].transform.parent);
    }

    protected override void SetItemProperties()
    {
        ItemActionHandler.RegisterMiddleClickAction(Info.TechType, item => GymbagManager.Instance.OnOpen(item), "open storage", "English");
    }
}
