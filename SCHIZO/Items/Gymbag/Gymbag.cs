using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Attributes;
using SCHIZO.Resources;
using SCHIZO.Unity.Items;

namespace SCHIZO.Items.Gymbag;

[LoadMethod]
public sealed class Gymbag : ItemPrefab
{
    private const TechType BagTechType = Retargeting.TechType.Bag;

    [LoadMethod]
    private static void Load()
    {
        new Gymbag(ModItems.Gymbag).Register();
    }

    [SetsRequiredMembers]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public Gymbag(ModItem modItem) : base(modItem)
    {
        ItemData = ResourceManager.LoadAsset<ItemData>("Gymbag data");
        Recipe = new RecipeData(new Ingredient(BagTechType, 1), new Ingredient(ModItems.Ermfish, 1), new Ingredient(TechType.PosterKitty, 1));
        SizeInInventory = new Vector2int(2, 2);
        TechGroup = TechGroup.Personal;
        TechCategory = TechCategory.Equipment;
        EquipmentType = EquipmentType.Hand;
        QuickSlotType = QuickSlotType.Selectable;
        RequiredForUnlock = ModItems.Ermfish;
        CloneTechType = BagTechType;
    }

    protected override void AddGadgets()
    {
        // make this an editor component?
        CraftingGadget crafting = GetGadget<CraftingGadget>();
        crafting.WithFabricatorType(CraftTree.Type.Fabricator);
        crafting.WithStepsToFabricatorTab(CraftTreeHandler.Paths.FabricatorEquipment);
        crafting.WithCraftingTime(10);
    }

    protected override void ModifyPrefab(GameObject prefab)
    {
        StorageContainer container = prefab.GetComponentInChildren<StorageContainer>();
        container.width = 4;
        container.height = 4;

        GameObject baseModel = prefab.GetComponentInChildren<Renderer>().gameObject;
        baseModel.SetActive(false);

        GameObject instance = Object.Instantiate(ItemData.prefab, baseModel.transform.parent);

        PrefabUtils.AddVFXFabricating(instance, null, 0, 0.93f, new Vector3(0, -0.05f), 0.75f, Vector3.zero);
    }

    protected override void PostRegister()
    {
        ItemActionHandler.RegisterMiddleClickAction(Info.TechType, item => GymbagBehaviour.Instance.OnOpen(item), "open storage", "English");
    }
}
