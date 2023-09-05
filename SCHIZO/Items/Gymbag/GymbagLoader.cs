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

namespace SCHIZO.Items.Gymbag;

public static class GymbagLoader
{
    public static void Load()
    {
        CustomPrefab prefab = new(ModItems.Gymbag);
        prefab.Info.WithSizeInInventory(new Vector2int(2, 2));
        prefab.Info.WithIcon(AssetLoader.GetAtlasSprite("gymbag.png"));

        prefab.SetGameObject(new CloneTemplate(prefab.Info, TechType.LuggageBag)
        {
            ModifyPrefab = ModifyPrefab
        });

        CraftingGadget crafting = prefab.SetRecipe(new RecipeData(new CraftData.Ingredient(TechType.LuggageBag), new CraftData.Ingredient(ModItems.Ermfish), new CraftData.Ingredient(TechType.PosterKitty)));
        crafting.WithFabricatorType(CraftTree.Type.Fabricator);
        crafting.WithStepsToFabricatorTab(CraftTreeHandler.Paths.FabricatorEquipment);
        crafting.WithCraftingTime(10);

        EquipmentGadget equipment = prefab.SetEquipment(EquipmentType.Hand);
        equipment.WithQuickSlotType(QuickSlotType.Selectable);

        prefab.SetUnlock(ModItems.Ermfish);
        prefab.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);

        prefab.Register();

        ItemActionHandler.RegisterMiddleClickAction(prefab.Info.TechType, item => GymbagHandler.main.OnOpen(item), "open storage", "English");
    }

    private static void ModifyPrefab(GameObject prefab)
    {
        StorageContainer container = prefab.GetComponentInChildren<StorageContainer>();
        container.width = 4;
        container.height = 4;

        GameObject carryallModel = prefab.GetComponentInChildren<MeshRenderer>().gameObject;
        carryallModel.SetActive(false);

        GameObject ourModel = AssetLoader.GetMainAssetBundle().LoadAssetSafe<GameObject>("gymbag");
        GameObject instance = GameObject.Instantiate(ourModel, carryallModel.transform.parent);

        CreaturePrefabUtils.AddVFXFabricating(instance, new VFXFabricatingData(null, 0, 0.93f, new Vector3(0, -0.05f), 0.75f, Vector3.zero));

        MaterialUtils.ApplySNShaders(instance);
    }
}
