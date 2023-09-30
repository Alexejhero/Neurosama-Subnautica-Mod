using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.Sounds;
using SCHIZO.Unity.Items;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SCHIZO.Buildables;

public sealed class BuildablePrefab : CustomPrefab
{
    private const string INDOOR_SOUNDS_BUS = "bus:/master/SFX_for_pause/PDA_pause/all/indoorsounds";

    public ItemData ItemData { get; init; }
    public TechGroup TechGroup { get; init; } = TechGroup.Uncategorized;
    public TechCategory TechCategory { get; init; }
    public RecipeData Recipe { get; init; }
    public TechType RequiredForUnlock { get; init; }
    public Action<GameObject> ModifyPrefab { get; init; } = _ => { };
    public bool DisableSounds { get; init; }

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

        if (ItemData.icon) Info.WithIcon(ItemData.icon);
        this.SetRecipe(Recipe);
        if (TechGroup != TechGroup.Uncategorized) this.SetPdaGroupCategory(TechGroup, TechCategory);
        if (RequiredForUnlock != TechType.None) this.SetUnlock(RequiredForUnlock);

        SetGameObject(GetPrefab);
        base.Register();
    }

    public BuildablePrefab WithOldVersion(string oldClassId)
    {
        if (_modItem == null) throw new InvalidOperationException($"Cannot add an old version to buildable which is already an old version (tying to add {oldClassId} to {Info.ClassID})");

        _oldVersions.Add(new BuildablePrefab(oldClassId, _modItem.DisplayName + " (OLD VERSION, PLEASE REBUILD)", _modItem.Tooltip + " (OLD VERSION, PLEASE REBUILD)")
        {
            ItemData = ItemData,
            Recipe = Recipe,
            DisableSounds = true,
        });

        return this;
    }

    private GameObject GetPrefab()
    {
        GameObject instance = Object.Instantiate(ItemData.prefab, BuildablesLoader.DisabledParent);
        PrefabUtils.AddBasicComponents(instance, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Medium);

        Transform child = instance.transform.GetChild(0); // each buildable should have an unique child with an appropriate collider

        Constructable con = PrefabUtils.AddConstructable(instance, Info.TechType, ConstructableFlags.Outside | ConstructableFlags.Base | ConstructableFlags.Submarine | ConstructableFlags.AllowedOnConstructable | ConstructableFlags.Ground | ConstructableFlags.Inside, child.gameObject);

        con.rotationEnabled = true;
        MaterialHelpers.FixBZGhostMaterial(con);

        if (!DisableSounds && ItemData.sounds) WorldSounds.Add(instance, new SoundPlayer(ItemData.sounds, INDOOR_SOUNDS_BUS));

        ModifyPrefab(instance);
        MaterialUtils.ApplySNShaders(instance, 1);

        return instance;
    }
}
