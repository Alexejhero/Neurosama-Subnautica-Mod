using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Crafting;
using Nautilus.Handlers;
using SCHIZO.Sounds;
using SCHIZO.Unity.Creatures;

namespace SCHIZO.Creatures;

public abstract class PickupableCreatureLoader<TData, TPrefab, TLoader> : CustomCreatureLoader<TData, TPrefab, TLoader>
    where TData : PickupableCreatureData
    where TPrefab : CreatureAsset, IPickupableCreaturePrefab
    where TLoader : PickupableCreatureLoader<TData, TPrefab, TLoader>, new()
{
    protected VFXFabricatingData VFXFabricatingData { get; init; }
    protected bool VariantsAreAlive { get; init; } = false;

    protected PickupableCreatureLoader(TData data) : base(data)
    {
    }

    public List<TechType> TechTypes { get; private set; }

    public override void Register()
    {
        base.Register();

        TechTypes = new List<TechType> { prefab.ModItem, prefab.CookedItem, prefab.CuredItem };
        prefab.PrefabInfo.WithIcon(creatureData.regularIcon);

        KnownTechHandler.SetAnalysisTechEntry(new KnownTech.AnalysisTech
        {
            techType = prefab.ModItem,
            unlockTechTypes = new List<TechType>(),
            unlockMessage = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredMessage,
            unlockSound = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredSound,
            unlockPopup = creatureData.unlockSprite
        });

        LoadVariants();
    }

    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    private void LoadVariants()
    {
        new CreatureVariant(prefab.ModItem, prefab.CookedItem)
        {
            IsAlive = VariantsAreAlive,
            Icon = creatureData.cookedIcon,
            RecipeData = new RecipeData(new Ingredient(prefab.ModItem, 1)),
            EdibleData = new EdibleData(prefab.FoodValueCooked, prefab.WaterValueCooked, true),
            FabricatorPath = CraftTreeHandler.Paths.FabricatorCookedFood,
            TechCategory = Retargeting.TechCategory.CookedFood,
            MaterialRemap = creatureData.cookedRemap,
            RegisterAsCookedVariant = true,
            VFXFabricatingData = VFXFabricatingData,
            PostRegister = VariantsAreAlive ? _ => PostRegisterAlive(prefab.CookedItem) : _ => PostRegisterDead(prefab.CookedItem),
        }.Register();

        new CreatureVariant(prefab.ModItem, prefab.CuredItem)
        {
            IsAlive = VariantsAreAlive,
            Icon = creatureData.curedIcon,
            RecipeData = new RecipeData(new Ingredient(prefab.ModItem, 1), new Ingredient(TechType.Salt, 1)),
            EdibleData = new EdibleData(prefab.FoodValueCooked, -3, false),
            FabricatorPath = CraftTreeHandler.Paths.FabricatorCuredFood,
            TechCategory = Retargeting.TechCategory.CuredFood,
            MaterialRemap = creatureData.curedRemap,
            VFXFabricatingData = VFXFabricatingData,
            PostRegister = VariantsAreAlive ? _ => PostRegisterAlive(prefab.CuredItem) : _ => PostRegisterDead(prefab.CuredItem),
        }.Register();
    }

    protected sealed override void PostRegister()
    {
        PostRegisterAlive(prefab.ModItem);
    }

    protected virtual void PostRegisterAlive(ModItem item)
    {
        CreatureSoundsHandler.RegisterCreatureSounds(item, creatureSounds);
    }

    protected virtual void PostRegisterDead(ModItem item)
    {

    }
}
