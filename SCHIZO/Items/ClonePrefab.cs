using System;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace SCHIZO.Items;

public abstract class ClonePrefab : UnityPrefab
{
    protected readonly TechType clonedTechType;

    [SetsRequiredMembers]
    protected ClonePrefab(ModItem item, TechType cloned) : base(item)
    {
        clonedTechType = cloned;
    }

    [Obsolete("This method is not supported on ClonePrefab.", true)]
    public new static void CreateAndRegister(ModItem item)
    {
        throw new NotSupportedException();
    }

    protected sealed override NautilusPrefabConvertible GetPrefab()
    {
        return new CloneTemplate(modItem, clonedTechType)
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
