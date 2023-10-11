using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using SCHIZO.Attributes;
using SCHIZO.Extensions;
using SCHIZO.Resources;
using SCHIZO.Unity.Items;
using UnityEngine;

namespace SCHIZO.Items.FumoItem;

[LoadMethod]
public class FumoItem : ItemPrefab
{
    [LoadMethod]
    public static void Load()
    {
        new FumoItem(ModItems.NeuroFumoItem).Register();
    }

    [SetsRequiredMembers]
    public FumoItem(ModItem modItem) : base(modItem)
    {
        ItemData = Assets.NeurofumoNew_FumoData;
        Recipe = new RecipeData(new Ingredient(TechType.CopperWire, 1), new Ingredient(TechType.Silicone, 2), new Ingredient(TechType.JeweledDiskPiece, 1), new Ingredient(TechType.Gold, 1));
        TechGroup = TechGroup.Personal;
        TechCategory = TechCategory.Equipment;
        EquipmentType = EquipmentType.Hand;
        QuickSlotType = QuickSlotType.Selectable;
    }

    protected override void AddGadgets()
    {
        CraftingGadget craft = GetGadget<CraftingGadget>();
        craft.WithFabricatorType(CraftTree.Type.Fabricator);
        craft.WithStepsToFabricatorTab(CraftTreeHandler.Paths.FabricatorEquipment);
        craft.WithCraftingTime(3);
    }

    protected override void ModifyPrefab(GameObject prefab)
    {
        Rigidbody rb = prefab.EnsureComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        Pickupable pickupable = prefab.EnsureComponent<Pickupable>();
        WorldForces gravity = prefab.EnsureComponent<WorldForces>();
        FumoItemTool tool = prefab.AddComponent<FumoItemTool>();
        tool.ikAimLeftArm = true;
        tool.ikAimRightArm = true;
        //tool.useLeftAimTargetOnPlayer = true;
        tool.leftHandIKTarget = prefab.transform.Find("VM/IK_LeftHand");
        tool.rightHandIKTarget = prefab.transform.Find("VM/IK_RightHand");
        //tool.mainCollider = prefab.GetComponentInChildren<Collider>();
#if BELOWZERO
        tool.ikAimLookDownAngleLimit = 30f;
#endif
        tool.pickupable = pickupable;
        FPModel fpModel = prefab.EnsureComponent<FPModel>();
        fpModel.propModel = prefab.transform.Find("WM").gameObject;
        fpModel.viewModel = prefab.transform.Find("VM").gameObject;

        prefab.EnsureComponentFields();
    }

    protected override void PostRegister()
    {
#if BELOWZERO
        CraftDataHandler.SetColdResistance(ModItems.NeuroFumoItem, 20);
#endif
    }
}
