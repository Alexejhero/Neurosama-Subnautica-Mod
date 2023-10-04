using System.Diagnostics.CodeAnalysis;
using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using SCHIZO.Creatures;
using SCHIZO.Helpers;
using SCHIZO.Unity.Items;
using UnityEngine;

namespace SCHIZO.Items;

public abstract class ItemPrefab : CustomPrefab, IItemRegisterer
{
    #region Keep alive

    private static readonly Transform _keepAliveParent;

    static ItemPrefab()
    {
        _keepAliveParent = new GameObject("KeepAlive").transform;
        _keepAliveParent.gameObject.SetActive(false);
        Object.DontDestroyOnLoad(_keepAliveParent);
    }

    #endregion

    #region Assignable properties

    protected ItemData ItemData { get; init; }
    protected TechGroup TechGroup { get; init; } = TechGroup.Uncategorized;
    protected TechCategory TechCategory { get; init; }
    protected RecipeData Recipe { get; init; }
    protected CraftTree.Type FabricatorType { get; init; } = CraftTree.Type.Fabricator;
    protected string[] FabricatorPath { get; init; }
    protected float CraftingTime { get; init; }
    protected Vector2int SizeInInventory { get; init; } = new(1, 1);
    protected TechType RequiredForUnlock { get; init; }
    protected EquipmentType EquipmentType { get; init; }
    protected QuickSlotType QuickSlotType { get; init; }
    protected LargeWorldEntity.CellLevel CellLevel { get; init; } = LargeWorldEntity.CellLevel.Near;
    protected TechType CloneTechType { get; init; }
    protected VFXFabricatingData VFXFabricatingData { get; init; }

    #endregion

    public ModItem ModItem { get; }

    [SetsRequiredMembers]
    public ItemPrefab(ModItem modItem) : base(modItem)
    {
        ModItem = modItem;
    }

    [SetsRequiredMembers]
    protected ItemPrefab(string classId, string displayName, string tooltip) : base(classId, displayName, tooltip)
    {
    }

    protected virtual void AddGadgets()
    {
    }

    protected virtual void ModifyPrefab(GameObject prefab)
    {
    }

    protected virtual void PostRegister()
    {
    }

    private void AddBasicGadgets()
    {
        if (ItemData!?.icon) Info.WithIcon(ItemData.icon);
        Info.WithSizeInInventory(SizeInInventory);

        CraftingGadget crafting = this.SetRecipe(Recipe);
        crafting.WithFabricatorType(FabricatorType);
        crafting.WithStepsToFabricatorTab(FabricatorPath);
        crafting.WithCraftingTime(CraftingTime);

        if (TechGroup != TechGroup.Uncategorized) this.SetPdaGroupCategory(TechGroup, TechCategory);
        if (RequiredForUnlock != TechType.None) this.SetUnlock(RequiredForUnlock);

        if (EquipmentType != EquipmentType.None)
            this.SetEquipment(EquipmentType).WithQuickSlotType(QuickSlotType);
    }

    public new virtual void Register()
    {
        AddBasicGadgets();
        AddGadgets();

        if (CloneTechType == TechType.None)
        {
            SetGameObject(GetPrefab);
        }
        else
        {
            SetGameObject(new CloneTemplate(Info, CloneTechType)
            {
                ModifyPrefab = ModifyPrefab,
            });
        }

        base.Register();
        PostRegister();
    }

    protected virtual GameObject GetPrefab()
    {
        GameObject instance = Object.Instantiate(ItemData.prefab, _keepAliveParent);
        PrefabUtils.AddBasicComponents(instance, Info.ClassID, Info.TechType, CellLevel);

        ModifyPrefab(instance);

        if (VFXFabricatingData != null) CreaturePrefabUtils.AddVFXFabricating(instance, VFXFabricatingData);

        CoroutineHelpers.RunWhen(() => MaterialHelpers.ApplySNShadersIncludingRemaps(instance, 1), () => MaterialHelpers.IsReady);

        return instance;
    }
}
