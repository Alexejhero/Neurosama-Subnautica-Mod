using System;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace SCHIZO.Buildables;

public sealed class BuildablePrefab : CustomPrefab
{
    public string IconFileName { get; init; }
    public TechGroup TechGroup { get; init; }
    public TechCategory TechCategory { get; init; }
    public RecipeData Recipe { get; init; }
    public string AssetBundleName { get; init; }
    public string PrefabName { get; init; }
    public Vector3 PrefabRotationEuler { get; init; } = Vector3.Zero;
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
        this.SetPdaGroupCategory(TechGroup, TechCategory);
        this.SetRecipe(Recipe);
        base.Register();
    }

    private GameObject GetPrefab()
    {
        GameObject prefab = AssetLoader.GetAssetBundle(AssetBundleName).LoadAsset<GameObject>(PrefabName);
        PrefabUtils.AddBasicComponents(prefab, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Medium);

        Transform child = prefab.transform.GetChild(0);
        child.Rotate(PrefabRotationEuler.X, PrefabRotationEuler.Y, PrefabRotationEuler.Z);

        Constructable con = PrefabUtils.AddConstructable(prefab, Info.TechType, ConstructableFlags.Outside | ConstructableFlags.Base | ConstructableFlags.Submarine | ConstructableFlags.AllowedOnConstructable | ConstructableFlags.Ground | ConstructableFlags.Inside, child.gameObject);

        MaterialUtils.ApplySNShaders(prefab, 1);
        prefab.transform.localScale *= PrefabScaleMultiplier;
        con.rotationEnabled = true;

        ModifyPrefab(prefab);

        return prefab;
    }
}
