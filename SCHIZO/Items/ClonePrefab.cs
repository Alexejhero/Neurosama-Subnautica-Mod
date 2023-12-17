using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace SCHIZO.Items;

public abstract class ClonePrefab : UnityPrefab
{
    protected readonly TechType clonedTechType;

    [SetsRequiredMembers]
    // ReSharper disable once ConvertToPrimaryConstructor
    protected ClonePrefab(ModItem item, TechType cloned) : base(item)
    {
        clonedTechType = cloned;
    }

    protected sealed override NautilusPrefabConvertible GetPrefab()
    {
        return new CloneTemplate(ModItem, clonedTechType)
        {
            ModifyPrefab = prefab =>
            {
                ModifyClone(prefab);
                ModifyPrefab(prefab);
            }
        };
    }

    protected virtual void ModifyClone(GameObject template)
    {
    }
}
