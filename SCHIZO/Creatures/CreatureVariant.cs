using System;
using System.Diagnostics.CodeAnalysis;
using ECCLibrary;
using ECCLibrary.Data;
using JetBrains.Annotations;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using SCHIZO.Gadgets;
using SCHIZO.Unity;

namespace SCHIZO.Creatures;

public sealed class CreatureVariant : CustomPrefab
{
    public string IconPath { get; init; }
    public RecipeData RecipeData { get; init; }
    public EdibleData EdibleData { get; init; }
    public string[] FabricatorPath { get; init; }
    public TechCategory TechCategory { [UsedImplicitly] get; init; }
    public string MaterialRemapName { get; init; }
    public bool RegisterAsCookedVariant { get; init; }
    public VFXFabricatingData VFXFabricatingData { get; init; }
    public Action<CreatureVariant> PostRegister { get; init; } = _ => { };

    private readonly TechType _original;

    [SetsRequiredMembers]
    public CreatureVariant(TechType original, PrefabInfo variant) : base(variant)
    {
        _original = original;
    }

    public new void Register()
    {
        Info.WithIcon(AssetLoader.GetAtlasSprite(IconPath));

        CraftingGadget crafting = this.SetRecipe(RecipeData);
        crafting.WithFabricatorType(CraftTree.Type.Fabricator);
        crafting.WithStepsToFabricatorTab(FabricatorPath);

        this.SetEdibleData(EdibleData);
        this.SetUnlock(_original);
        this.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);

#if SUBNAUTICA
        this.SetPdaGroupCategory(TechGroup.Survival, TechCategory);
#endif

        SetGameObject(new CloneTemplate(Info, _original)
        {
            ModifyPrefab = prefab =>
            {
                prefab.GetComponentsInChildren<MaterialRemapper>().ApplyAll(MaterialRemapName);
                CreaturePrefabUtils.AddVFXFabricating(prefab, VFXFabricatingData);
            }
        });
        base.Register();

        if (RegisterAsCookedVariant) CraftDataHandler.SetCookedVariant(_original, Info.TechType);

        PostRegister?.Invoke(this);
    }
}
