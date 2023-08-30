using System;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;

namespace SCHIZO.Buildables;

public sealed class BuildablePrefab : CustomPrefab
{
    public string IconFileName { get; init; }
    public TechGroup TechGroup { get; init; } = TechGroup.Uncategorized;
    public TechCategory TechCategory { get; init; }
    public RecipeData Recipe { get; init; }
    public string AssetBundleName { get; init; }
    public string PrefabName { get; init; }
    public Vector3 PrefabRotationEuler { get; init; } = Vector3.zero;
    public float PrefabScaleMultiplier { get; init; } = 1;
    public Action<GameObject> ModifyPrefab { get; init; } = _ => { };

    [SetsRequiredMembers]
    public BuildablePrefab(string classId, string friendlyName, string description) : base(classId, friendlyName, description)
    {
    }

    public new void Register()
    {
        SetGameObject(GetPrefab);
        Info.WithIcon(AssetLoader.GetAtlasSprite(IconFileName));
        if (TechGroup != TechGroup.Uncategorized) this.SetPdaGroupCategory(TechGroup, TechCategory);
        this.SetRecipe(Recipe);
        base.Register();
    }

    private GameObject GetPrefab()
    {
        GameObject prefab = AssetLoader.GetAssetBundle(AssetBundleName).LoadAsset<GameObject>(PrefabName);
        GameObject instance = GameObject.Instantiate(prefab, BuildablesLoader.DisabledParent);
        PrefabUtils.AddBasicComponents(instance, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Medium);

        Transform child = instance.transform.GetChild(0);
        child.Rotate(PrefabRotationEuler.x, PrefabRotationEuler.y, PrefabRotationEuler.z);

        Constructable con = PrefabUtils.AddConstructable(instance, Info.TechType, ConstructableFlags.Outside | ConstructableFlags.Base | ConstructableFlags.Submarine | ConstructableFlags.AllowedOnConstructable | ConstructableFlags.Ground | ConstructableFlags.Inside, child.gameObject);

        instance.transform.localScale *= PrefabScaleMultiplier;
        con.rotationEnabled = true;

        ModifyPrefab(instance);
        MaterialUtils.ApplySNShaders(instance, 1);

        return instance;
    }
}
