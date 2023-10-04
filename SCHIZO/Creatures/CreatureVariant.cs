using System;
using System.Diagnostics.CodeAnalysis;
using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using SCHIZO.Gadgets;
using SCHIZO.Items;
using SCHIZO.Unity.Materials;
using UnityEngine;

namespace SCHIZO.Creatures;

public sealed class CreatureVariant : ItemPrefab
{
    #region Exposing protected properties

    public new VFXFabricatingData VFXFabricatingData
    {
        private get => base.VFXFabricatingData;
        init => base.VFXFabricatingData = value;
    }

    #endregion

    public bool IsAlive { get; init; } // TODO: Implement variants not being alive
    public Sprite Icon { get; init; }
    public MaterialRemapOverride MaterialRemap { get; init; }
    public Action<ModItem> PostRegisterFunc { get; init; }

    private readonly Type _variantType;
    private readonly IPickupableCreaturePrefab _original;
    private readonly EdibleData _edibleData;

    [SetsRequiredMembers]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public CreatureVariant(IPickupableCreaturePrefab prefab, ModItem variant, Type variantType) : base(variant)
    {
        _original = prefab;
        _variantType = variantType;
        RequiredForUnlock = _original.ModItem;
        EquipmentType = EquipmentType.Hand;
        QuickSlotType = QuickSlotType.Selectable;
        TechGroup = IS_BELOWZERO ? TechGroup.Uncategorized : TechGroup.Survival;

        if (_variantType == Type.Cooked)
        {
            Recipe = new RecipeData(new Ingredient(prefab.ModItem, 1));
            FabricatorPath = CraftTreeHandler.Paths.FabricatorCookedFood;
            TechCategory = Retargeting.TechCategory.CookedFood;

            _edibleData = new EdibleData(prefab.FoodValueCooked, prefab.WaterValueCooked, true, 1);
        }
        else if (_variantType == Type.Cured)
        {
            Recipe = new RecipeData(new Ingredient(prefab.ModItem, 1), new Ingredient(TechType.Salt, 1));
            FabricatorPath = CraftTreeHandler.Paths.FabricatorCuredFood;
            TechCategory = Retargeting.TechCategory.CuredFood;

            _edibleData = new EdibleData(prefab.FoodValueCooked, -3, false);
        }
    }

    public override void Register()
    {
        Info.WithIcon(Icon);

        this.SetEdibleData(_edibleData);

        if (_variantType == Type.Cooked) CraftDataHandler.SetCookedVariant(_original.ModItem, Info.TechType);

#if BELOWZERO
        CraftDataHandler.SetSoundType(Info.TechType, TechData.GetSoundType(_original.ModItem));
#endif

        SetGameObject(new CloneTemplate(Info, _original.ModItem)
        {
            ModifyPrefab = prefab =>
            {
                // we're not actually applying the remap itself, instead we are applying the Single remap that we find with that name
                // this is so that if we have remappers with different configs, this won't break
                // however i don't think that^ should ever happen anyway
                prefab.GetComponentsInChildren<MaterialRemapper>().ApplyAll(MaterialRemap);
                CreaturePrefabUtils.AddVFXFabricating(prefab, VFXFabricatingData);
            }
        });

        base.Register();
    }

    protected override void PostRegister()
    {
        PostRegisterFunc?.Invoke(modItem);
    }
}
