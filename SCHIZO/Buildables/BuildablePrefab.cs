using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using SCHIZO.DataStructures;
using SCHIZO.Extensions;
using UnityEngine;

namespace SCHIZO.Buildables;

public sealed class BuildablePrefab : CustomPrefab
{
    public string IconFileName { get; init; }
    public TechGroup TechGroup { get; init; } = TechGroup.Uncategorized;
    public TechCategory TechCategory { get; init; }
    public RecipeData Recipe { get; init; }
    public string PrefabName { get; init; }
    public Action<GameObject> ModifyPrefab { get; init; } = _ => { };

    private readonly ModItem _modItem;
    private readonly List<BuildablePrefab> _oldVersions = new();

    [SetsRequiredMembers]
    public BuildablePrefab(ModItem item) : base(item)
    {
        _modItem = item;
    }

    [SetsRequiredMembers]
    private BuildablePrefab(string classId, string displayName, string tooltip) : base(classId, displayName, tooltip)
    {
    }

    public new void Register()
    {
        _oldVersions.ForEach(v => v.Register());

        SetGameObject(GetPrefab);
        Info.WithIcon(AssetLoader.GetAtlasSprite(IconFileName));
        if (TechGroup != TechGroup.Uncategorized) this.SetPdaGroupCategory(TechGroup, TechCategory);
        this.SetRecipe(Recipe);
        base.Register();
    }

    public BuildablePrefab WithOldVersion(string oldClassId)
    {
        if (_modItem == null) throw new InvalidOperationException($"Cannot add an old version to buildable which is already an old version (tying to add {oldClassId} to {Info.ClassID})");

        _oldVersions.Add(new BuildablePrefab(oldClassId, _modItem.DisplayName + " (OLD VERSION, PLEASE REBUILD)", _modItem.Tooltip + " (OLD VERSION, PLEASE REBUILD)")
        {
            IconFileName = IconFileName,
            Recipe = Recipe,
            PrefabName = PrefabName,
        });

        return this;
    }

    private GameObject GetPrefab()
    {
        GameObject prefab = AssetLoader.GetMainAssetBundle().LoadAssetSafe<GameObject>(PrefabName);
        GameObject instance = GameObject.Instantiate(prefab, BuildablesLoader.DisabledParent);
        PrefabUtils.AddBasicComponents(instance, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Medium);

        Transform child = instance.transform.GetChild(0); // each buildable should have an unique child with an appropriate collider

        Constructable con = PrefabUtils.AddConstructable(instance, Info.TechType, ConstructableFlags.Outside | ConstructableFlags.Base | ConstructableFlags.Submarine | ConstructableFlags.AllowedOnConstructable | ConstructableFlags.Ground | ConstructableFlags.Inside, child.gameObject);

        con.rotationEnabled = true;

        ModifyPrefab(instance);
        MaterialUtils.ApplySNShaders(instance, 1);

        return instance;
    }
}
