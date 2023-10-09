using System.Diagnostics.CodeAnalysis;
using Nautilus.Utility;
using SCHIZO.Unity.Items;
using UnityEngine;

namespace SCHIZO.BuildablesV2;

public sealed class SealedBuildablePrefab : BaseCustomPrefab
{
    [SetsRequiredMembers]
    public SealedBuildablePrefab(ModItem modItem, ItemData data) : base(modItem, data)
    {
    }

    protected override GameObject GetPrefab()
    {
        GameObject prefab = base.GetPrefab();

        TechTag techTag = prefab.GetComponent<TechTag>();
        Constructable constructable = prefab.GetComponent<Constructable>();

        LOGGER.LogWarning("READY: " + MaterialUtils.IsReady);

        constructable.techType = techTag.type;
        constructable.ghostMaterial = MaterialUtils.GhostMaterial;

        // TODO: more stuff for bz
        // constructable._tex

        return prefab;
    }
}
