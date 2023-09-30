using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using SCHIZO.Helpers;
using SCHIZO.Unity.Items;

namespace SCHIZO.Items;

public class ItemPrefab : CustomPrefab
{
    private static Transform KeepAliveParent { get; } = new GameObject("KeepAlive").transform;

    static ItemPrefab()
    {
        KeepAliveParent.gameObject.SetActive(false);
        Object.DontDestroyOnLoad(KeepAliveParent);
    }

    public ItemData ItemData { get; init; }
    public TechGroup TechGroup { get; init; } = TechGroup.Uncategorized;
    public TechCategory TechCategory { get; init; }
    public RecipeData Recipe { get; init; }
    public Vector2int SizeInInventory { get; init; } = new(1, 1);
    public TechType RequiredForUnlock { get; init; }
    public EquipmentType EquipmentType { get; init; } = EquipmentType.Hand;
    public QuickSlotType QuickSlotType { get; init; } = QuickSlotType.Selectable;
    public LargeWorldEntity.CellLevel CellLevel { get; init; } = LargeWorldEntity.CellLevel.Near;
    public TechType CloneTechType { get; init; }

    // to keep object initializers; in case of dissatisfaction please visit https://github.com/dotnet/csharplang/issues/5176
    public Action AddGadgets { init => OnAddGadgets += value; }
    public event Action OnAddGadgets;

    public Action<GameObject> ModifyPrefab { init => OnModifyPrefab += value; }
    public event Action<GameObject> OnModifyPrefab;

    public Action PostRegister { init => OnPostRegister += value; }
    public event Action OnPostRegister;


    protected readonly ModItem _modItem;

    [SetsRequiredMembers]
    public ItemPrefab(ModItem modItem) : base(modItem)
    {
        _modItem = modItem;
    }

    [SetsRequiredMembers]
    protected ItemPrefab(string classId, string displayName, string tooltip) : base(classId, displayName, tooltip)
    {
    }

    private void AddBasicGadgets()
    {
        if (ItemData?.icon) Info.WithIcon(ItemData.icon);
        Info.WithSizeInInventory(SizeInInventory);
        this.SetRecipe(Recipe);
        if (TechGroup != TechGroup.Uncategorized) this.SetPdaGroupCategory(TechGroup, TechCategory);
        if (RequiredForUnlock != TechType.None) this.SetUnlock(RequiredForUnlock);

        if (EquipmentType != EquipmentType.None)
            this.SetEquipment(EquipmentType).WithQuickSlotType(QuickSlotType);
    }

    public new virtual void Register()
    {
        AddBasicGadgets();
        OnAddGadgets?.Invoke();

        if (CloneTechType == TechType.None)
            SetGameObject(GetPrefab);
        else
        {
            SetGameObject(new CloneTemplate(Info, CloneTechType)
            {
                ModifyPrefab = OnModifyPrefab,
            });
        }

        base.Register();
        OnPostRegister?.Invoke();
    }

    protected virtual GameObject GetPrefab()
    {
        GameObject instance = Object.Instantiate(ItemData.prefab, KeepAliveParent);
        PrefabUtils.AddBasicComponents(instance, Info.ClassID, Info.TechType, CellLevel);

        OnModifyPrefab?.Invoke(instance);

        CoroutineHelpers.RunWhen(() => MaterialHelpers.ApplySNShadersIncludingRemaps(instance, 1), () => MaterialHelpers.IsReady);

        return instance;
    }
}
