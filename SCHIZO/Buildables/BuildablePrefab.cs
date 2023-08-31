using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using SCHIZO.Utilities;
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

    private readonly string _friendlyName;
    private readonly string _description;
    private readonly List<BuildablePrefab> _oldVersions = new();

    [SetsRequiredMembers]
    public BuildablePrefab(string classId, string friendlyName, string description) : base(classId, friendlyName, description)
    {
        _friendlyName = friendlyName;
        _description = description;
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
        _oldVersions.Add(new BuildablePrefab(oldClassId, _friendlyName + " (OLD VERSION, PLEASE REBUILD)", _description + " (OLD VERSION, PLEASE REBUILD)")
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

        Transform child = instance.transform.GetChild(0);

        Constructable con = PrefabUtils.AddConstructable(instance, Info.TechType, ConstructableFlags.Outside | ConstructableFlags.Base | ConstructableFlags.Submarine | ConstructableFlags.AllowedOnConstructable | ConstructableFlags.Ground | ConstructableFlags.Inside, child.gameObject);

        con.rotationEnabled = true;

        ModifyPrefab(instance);
        MaterialUtils.ApplySNShaders(instance, 1);

        return instance;
    }
}
