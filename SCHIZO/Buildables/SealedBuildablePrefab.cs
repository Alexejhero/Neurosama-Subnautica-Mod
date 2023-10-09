using System.Diagnostics.CodeAnalysis;
using Nautilus.Utility;
using SCHIZO.Items;
using SCHIZO.Unity.Items;
using UnityEngine;

namespace SCHIZO.Buildables;

public sealed class SealedBuildablePrefab : BaseCustomPrefab
{
    [SetsRequiredMembers]
    public SealedBuildablePrefab(ModItem modItem) : base(modItem)
    {
    }

    protected override GameObject GetPrefab()
    {
        GameObject prefab = base.GetPrefab();

        TechTag techTag = prefab.GetComponent<TechTag>();
        Constructable constructable = prefab.GetComponent<Constructable>();

        constructable.techType = techTag.type;
        constructable.ghostMaterial = MaterialUtils.GhostMaterial;

        // TODO: more stuff for bz
        // constructable._tex

        return prefab;
    }
}
