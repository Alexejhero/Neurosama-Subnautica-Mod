using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using SCHIZO.Helpers;
using SCHIZO.Unity.Items;

namespace SCHIZO.Items;

public abstract class ItemPrefab : CustomPrefab
{
    private static readonly Transform _keepAliveParent;

    static ItemPrefab()
    {
        _keepAliveParent = new GameObject("KeepAlive").transform;
        _keepAliveParent.gameObject.SetActive(false);
        Object.DontDestroyOnLoad(_keepAliveParent);
    }

    public ItemData UnityItemData { get; init; }
    public TechGroup TechGroup { get; init; } = TechGroup.Uncategorized;
    public TechCategory TechCategory { get; init; }
    public RecipeData Recipe { get; init; }
    public CraftTree.Type FabricatorType { get; init; }
    public string[] FabricatorPath { get; init; }
    public float CraftingTime { get; init; }
    public Vector2Int SizeInInventory { get; init; } = new(1, 1);
    public TechType RequiredForUnlock { get; init; }
    public EquipmentType EquipmentType { get; init; }
    public QuickSlotType QuickSlotType { get; init; }
    public LargeWorldEntity.CellLevel CellLevel { get; init; } = LargeWorldEntity.CellLevel.Near;
    public TechType CloneTechType { get; init; }

    protected readonly ModItem modItem;

    [SetsRequiredMembers]
    public ItemPrefab(ModItem modItem) : base(modItem)
    {
        this.modItem = modItem;
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
        if (UnityItemData!?.icon) Info.WithIcon(UnityItemData.icon);
        Info.WithSizeInInventory(new Vector2int(SizeInInventory.x, SizeInInventory.y));

        CraftingGadget crafting = this.SetRecipe(Recipe);
        if (FabricatorType != CraftTree.Type.None)
        {
            crafting.WithFabricatorType(FabricatorType);
            crafting.WithStepsToFabricatorTab(FabricatorPath);
            crafting.WithCraftingTime(CraftingTime);
        }

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
            SetGameObject(GetPrefab);
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
        GameObject instance = Object.Instantiate(UnityItemData.prefab, _keepAliveParent);
        PrefabUtils.AddBasicComponents(instance, Info.ClassID, Info.TechType, CellLevel);

        ModifyPrefab(instance);

        CoroutineHelpers.RunWhen(() => MaterialHelpers.ApplySNShadersIncludingRemaps(instance, 1), () => MaterialHelpers.IsReady);

        return instance;
    }
}
