using System;
using System.Diagnostics.CodeAnalysis;
using ECCLibrary;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using UnityEngine;

namespace SCHIZO.Creatures;

public sealed class PickupableCreatureDeadPrefab : CookedCreatureHandler.CookedCreatureTemplate, IItemRegisterer
{
    public ModItem ModItem { get; }
    public CreatureVariantType Type { get; }

    private readonly ModItem _parent;
    private readonly float _bioReactorCharge;

    [SetsRequiredMembers]
    public PickupableCreatureDeadPrefab(ModItem parent, ModItem variant, GameObject rawObject, CreatureVariantType type, IPickupableCreatureLoader loader)
        : base(variant, rawObject, loader.GenerateEdibleData(type), loader.VFXFabricatingData)
    {
        ModItem = variant;
        Type = type;
        _parent = parent;
        _bioReactorCharge = loader.BioReactorCharge;
    }

    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public void Register()
    {
        CustomPrefab customPrefab = new(ModItem);

        switch (Type)
        {
            case CreatureVariantType.Cooked:
                customPrefab.SetRecipe(new RecipeData(new Ingredient(_parent, 1)))
                    .WithFabricatorType(CraftTree.Type.Fabricator).WithStepsToFabricatorTab(CraftTreeHandler.Paths.FabricatorCookedFood);
                customPrefab.SetPdaGroupCategory(IS_BELOWZERO ? TechGroup.Uncategorized : TechGroup.Survival, Retargeting.TechCategory.CookedFood);
                break;

            case CreatureVariantType.Cured:
                customPrefab.SetRecipe(new RecipeData(new Ingredient(_parent, 1), new Ingredient(TechType.Salt, 1)))
                    .WithFabricatorType(CraftTree.Type.Fabricator).WithStepsToFabricatorTab(CraftTreeHandler.Paths.FabricatorCuredFood);
                customPrefab.SetPdaGroupCategory(IS_BELOWZERO ? TechGroup.Uncategorized : TechGroup.Survival, Retargeting.TechCategory.CuredFood);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        customPrefab.SetGameObject(this);
        customPrefab.Register();

        CreatureDataUtils.SetBioreactorCharge(ModItem, _bioReactorCharge);
    }
}
