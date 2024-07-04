using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace SCHIZO.Items;

[method: SetsRequiredMembers]
public abstract class ClonePrefab(ModItem item, TechType cloned) : UnityPrefab(item)
{
    protected readonly TechType clonedTechType = cloned;

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
