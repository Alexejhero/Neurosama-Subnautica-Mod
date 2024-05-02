using System.Diagnostics.CodeAnalysis;
using Nautilus.Handlers;
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

    public static readonly string GymbagStorageLabel = IS_BELOWZERO ? "QUANTUM GYMBAG" : "GYMBAG";
    [SetsRequiredMembers]
    // ReSharper disable once ConvertToPrimaryConstructor
    public Gymbag(ModItem modItem) : base(modItem, CLONE_TARGET)
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

        Object.Instantiate(UnityData.prefab, renderers[0].transform.parent);
    }

    protected override void SetItemProperties()
    {
        base.SetItemProperties();
        ItemActionHandler.RegisterMiddleClickAction(Info.TechType, item => GymbagManager.Instance.OnOpen(item), "open storage", "English");
    }
}
