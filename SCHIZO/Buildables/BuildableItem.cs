using Nautilus.Utility;
using SCHIZO.Gadgets;
using SCHIZO.Helpers;
using SCHIZO.Items;

namespace SCHIZO.Buildables;

public sealed class BuildableItem : ItemPrefab
{
    private const string INDOOR_SOUNDS_BUS = "bus:/master/SFX_for_pause/PDA_pause/all/indoorsounds";

    public bool DisableSounds { get; init; }

    private readonly List<BuildableItem> _oldVersions = new();
    public ConstructableFlags PlacementFlags { get; init; } = ConstructableFlags.Ground | ConstructableFlags.Inside | ConstructableFlags.Outside | ConstructableFlags.AllowedOnConstructable | ConstructableFlags.Rotatable;

    [SetsRequiredMembers]
    public BuildableItem(ModItem item) : base(item)
    {
        CellLevel = LargeWorldEntity.CellLevel.Medium;
    }

    [SetsRequiredMembers]
    private BuildableItem(string classId, string displayName, string tooltip) : base(classId, displayName, tooltip)
    {
        CellLevel = LargeWorldEntity.CellLevel.Medium;
    }

    public BuildableItem WithOldVersion(string oldClassId)
    {
        if (_modItem == null) throw new InvalidOperationException($"Cannot add an old version to buildable which is already an old version (tying to add {oldClassId} to {Info.ClassID})");

        _oldVersions.Add(new BuildableItem(oldClassId, _modItem.DisplayName + " (OLD VERSION, PLEASE REBUILD)", _modItem.Tooltip + " (OLD VERSION, PLEASE REBUILD)")
        {
            ItemData = ItemData,
            Recipe = Recipe,
            DisableSounds = true,
        });

        return this;
    }

    protected override void AddGadgets()
    {
        if (!DisableSounds && ItemData.sounds) this.SetSounds(new(ItemData.sounds, INDOOR_SOUNDS_BUS));
    }

    protected override void ModifyPrefab(GameObject prefab)
    {
        Transform child = prefab.transform.GetChild(0); // each buildable should have an unique child with an appropriate collider

        Constructable con = PrefabUtils.AddConstructable(prefab, Info.TechType, PlacementFlags, child.gameObject);

        con.rotationEnabled = true;
        MaterialHelpers.FixBZGhostMaterial(con);
    }

    public override void Register()
    {
        _oldVersions.ForEach(v => v.Register());

        base.Register();
    }
}
