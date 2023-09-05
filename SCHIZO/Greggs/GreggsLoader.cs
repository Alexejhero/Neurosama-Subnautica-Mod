using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Extensions;
using UnityEngine;

namespace SCHIZO.Greggs;

public static class GreggsLoader
{
    public static void Load()
    {
        CustomPrefab prefab = new(ModItems.Greggs);
        prefab.Info.WithSizeInInventory(new Vector2int(2, 2));
        prefab.Info.WithIcon(AssetLoader.GetAtlasSprite("greggs.png"));


        CraftingGadget crafting = new(prefab, new RecipeData(new CraftData.Ingredient(ModItems.DeadErmfish)));
        crafting.WithFabricatorType(CraftTree.Type.Fabricator);
        crafting.WithStepsToFabricatorTab(CraftTreeHandler.Paths.FabricatorCookedFood);
        crafting.WithCraftingTime(999);
        prefab.AddGadget(crafting);

        prefab.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);

        prefab.Register();
        }
}
