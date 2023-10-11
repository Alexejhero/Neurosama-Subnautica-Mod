using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace SCHIZO.Items;

public abstract class ClonePrefab : UnityPrefab
{
    private readonly TechType _original;

    [SetsRequiredMembers]
    protected ClonePrefab(ModItem item, TechType original) : base(item)
    {
        _original = original;
    }

    [Obsolete("This method is not supported on ClonePrefab.", true)]
    public new static void CreateAndRegister(ModItem item)
    {
        throw new NotSupportedException();
    }

    protected sealed override NautilusPrefabConvertible GetPrefab()
    {
        return new CloneTemplate(modItem, _original)
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
