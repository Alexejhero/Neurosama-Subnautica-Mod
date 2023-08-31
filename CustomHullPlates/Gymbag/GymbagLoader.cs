using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Creatures.Ermfish;
using SCHIZO.Utilities;
using UnityEngine;

namespace SCHIZO.Gymbag;

public static class GymbagLoader
{
    public static void Load()
    {
        CustomPrefab prefab = new(GymbagTypes.Gymbag);
        prefab.Info.WithSizeInInventory(new Vector2int(2, 2));
        prefab.Info.WithIcon(AssetLoader.GetAtlasSprite("gymbag.png"));

        prefab.SetGameObject(new CloneTemplate(prefab.Info, TechType.LuggageBag)
        {
            ModifyPrefab = ModifyPrefab
        });

        CraftingGadget crafting = new(prefab, new RecipeData(new CraftData.Ingredient(TechType.LuggageBag), new CraftData.Ingredient(ErmfishTypes.Regular.TechType)));
        crafting.WithFabricatorType(CraftTree.Type.Fabricator);
        crafting.WithStepsToFabricatorTab(CraftTreeHandler.Paths.FabricatorEquipment);
        prefab.AddGadget(crafting);

        prefab.SetEquipment(EquipmentType.Hand);

        prefab.Register();

        ItemActionHandler.RegisterMiddleClickAction(prefab.Info.TechType, item => GymbagHandler.main.OnOpen(item), "open storage", "English");
    }

    private static void ModifyPrefab(GameObject prefab)
    {
        StorageContainer container = prefab.GetComponentInChildren<StorageContainer>();
        container.width = 5;
        container.height = 4;

        GameObject carryallModel = prefab.GetComponentInChildren<MeshRenderer>().gameObject;
        carryallModel.SetActive(false);

        GameObject ourModel = AssetLoader.GetMainAssetBundle().LoadAssetSafe<GameObject>("gymbag");
        GameObject instance = GameObject.Instantiate(ourModel, carryallModel.transform.parent);

        MaterialUtils.ApplySNShaders(instance);
    }
}
