using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Crafting;
using Nautilus.Handlers;
using SCHIZO.Sounds;
using SCHIZO.Unity.Creatures;
using UnityEngine;

namespace SCHIZO.Creatures;

public abstract class PickupableCreatureLoader<TData, TDeadPrefab, TAlivePrefab, TLoader> : CustomCreatureLoader<TData, TAlivePrefab, TLoader>, IPickupableCreatureLoader
    where TData : PickupableCreatureData
    where TAlivePrefab : CreatureAsset, IItemRegisterer
    where TDeadPrefab : IItemRegisterer
    where TLoader : PickupableCreatureLoader<TData, TDeadPrefab, TAlivePrefab, TLoader>, new()
{
    protected float FoodValueRaw { get; init; }
    protected float WaterValueRaw { get; init; }
    protected float FoodValueCooked { get; init; }
    protected float WaterValueCooked { get; init; }
    public float BioReactorCharge { get; init; }
    public VFXFabricatingData VFXFabricatingData { get; init; }
    protected bool VariantsAreAlive { get; init; }

    protected PickupableCreatureLoader(TData data) : base(data)
    {
    }

    protected abstract TAlivePrefab CreateAlivePrefab(GameObject rawObject, CreatureVariantType type);
    protected abstract TDeadPrefab CreateDeadPrefab(GameObject rawObject, CreatureVariantType type);

    public override void Register()
    {
        base.Register();

        // TechTypes = new List<TechType> { creatureData.ModItem, regularPrefab.CookedItem, regularPrefab.CuredItem };
        regularPrefab.PrefabInfo.WithIcon(creatureData.regularIcon);

        KnownTechHandler.SetAnalysisTechEntry(new KnownTech.AnalysisTech
        {
            techType = regularPrefab.TechType,
            unlockTechTypes = new List<TechType>(),
            unlockMessage = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredMessage,
            unlockSound = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredSound,
            unlockPopup = creatureData.unlockSprite
        });

        if (VariantsAreAlive)
        {
            RegisterAlivePrefab(creatureData.cookedPrefab, CreatureVariantType.Cooked);
            RegisterAlivePrefab(creatureData.curedPrefab, CreatureVariantType.Cured);
        }
        else
        {
            RegisterDeadPrefab(creatureData.cookedPrefab, CreatureVariantType.Cooked);
            RegisterDeadPrefab(creatureData.curedPrefab, CreatureVariantType.Cured);
        }
    }

    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    private void LoadVariants()
    {
        new CreatureVariant(regularPrefab.ModItem, regularPrefab.CookedItem)
        {
            IsAlive = VariantsAreAlive,
            Icon = creatureData.cookedIcon,
            RecipeData = new RecipeData(new Ingredient(regularPrefab.ModItem, 1)),
            EdibleData = new EdibleData(regularPrefab.FoodValueCooked, regularPrefab.WaterValueCooked, true),
            FabricatorPath = CraftTreeHandler.Paths.FabricatorCookedFood,
            TechCategory = Retargeting.TechCategory.CookedFood,
            MaterialRemap = creatureData.cookedRemap,
            RegisterAsCookedVariant = true,
            VFXFabricatingData = VFXFabricatingData,
            PostRegister = VariantsAreAlive ? _ => PostRegisterAlive(regularPrefab.CookedItem) : _ => PostRegisterDead(regularPrefab.CookedItem),
        }.Register();

        new CreatureVariant(regularPrefab.ModItem, regularPrefab.CuredItem)
        {
            IsAlive = VariantsAreAlive,
            Icon = creatureData.curedIcon,
            RecipeData = new RecipeData(new Ingredient(regularPrefab.ModItem, 1), new Ingredient(TechType.Salt, 1)),
            EdibleData = new EdibleData(regularPrefab.FoodValueCooked, -3, false),
            FabricatorPath = CraftTreeHandler.Paths.FabricatorCuredFood,
            TechCategory = Retargeting.TechCategory.CuredFood,
            MaterialRemap = creatureData.curedRemap,
            VFXFabricatingData = VFXFabricatingData,
            PostRegister = VariantsAreAlive ? _ => PostRegisterAlive(regularPrefab.CuredItem) : _ => PostRegisterDead(regularPrefab.CuredItem),
        }.Register();
    }

    protected virtual void PostRegisterAlive(TAlivePrefab prefab)
    {
        CreatureSoundsHandler.RegisterCreatureSounds(prefab.ModItem, creatureSounds);
    }

    protected virtual void PostRegisterDead(TDeadPrefab prefab)
    {
    }

    public EdibleData GenerateEdibleData(CreatureVariantType type)
    {
        return type switch
        {
            CreatureVariantType.Regular => new EdibleData(FoodValueRaw, WaterValueRaw, false),
            CreatureVariantType.Cooked => new EdibleData(FoodValueCooked, WaterValueCooked, true, 1),
            CreatureVariantType.Cured => new EdibleData(FoodValueCooked, -3, false),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private TAlivePrefab RegisterAlivePrefab(GameObject rawObject, CreatureVariantType type)
    {
        TAlivePrefab creaturePrefab = CreateAlivePrefab(rawObject, type);
        ((IItemRegisterer) creaturePrefab).Register();

        TechTypes.Add(creaturePrefab.TechType);

        CreatureSoundsHandler.RegisterCreatureSounds(creaturePrefab.ModItem, creatureSounds);

        PostRegister(creaturePrefab);

        return creaturePrefab;
    }

    private TDeadPrefab RegisterDeadPrefab(GameObject rawObject, CreatureVariantType type)
    {
        TDeadPrefab creaturePrefab = CreateDeadPrefab(rawObject, type);
        ((IItemRegisterer) creaturePrefab).Register();

        TechTypes.Add(creaturePrefab.ModItem);

        PostRegisterDead(creaturePrefab);

        return creaturePrefab;
    }

    #region Redirecting base calls

    protected sealed override TAlivePrefab CreatePrefab(GameObject rawObject)
    {
        return CreateAlivePrefab(rawObject, CreatureVariantType.Regular);
    }

    protected sealed override void PostRegister(TAlivePrefab prefab)
    {
        PostRegisterAlive(prefab);
    }

    #endregion
}

public interface IPickupableCreatureLoader : ICustomCreatureLoader
{
    EdibleData GenerateEdibleData(CreatureVariantType type);

    float BioReactorCharge { get; }
    VFXFabricatingData VFXFabricatingData { get; }
}
