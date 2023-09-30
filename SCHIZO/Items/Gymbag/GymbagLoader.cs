using System;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Attributes;
using SCHIZO.Resources;
using SCHIZO.Unity.Items;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SCHIZO.Items.Gymbag;

[LoadMethod]
public static class GymbagLoader
{
    private static readonly TechType BagTechType =
#if BELOWZERO
        TechType.QuantumLocker;
#else
        TechType.LuggageBag;
#endif

    [LoadMethod]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    private static void Load()
    {
        ItemData data = ResourceManager.LoadAsset<ItemData>("Gymbag data");

        CustomPrefab prefab = new(ModItems.Gymbag);
        prefab.Info.WithSizeInInventory(new Vector2int(2, 2));
        prefab.Info.WithIcon(data.icon);

        prefab.SetGameObject(new CloneTemplate(prefab.Info, BagTechType)
        {
            ModifyPrefab = ModifyPrefab(data)
        });

        CraftingGadget crafting = prefab.SetRecipe(new RecipeData(new Ingredient(BagTechType, 1), new Ingredient(ModItems.Ermfish, 1), new Ingredient(TechType.PosterKitty, 1)));
        crafting.WithFabricatorType(CraftTree.Type.Fabricator);
        crafting.WithStepsToFabricatorTab(CraftTreeHandler.Paths.FabricatorEquipment);
        crafting.WithCraftingTime(10);

        EquipmentGadget equipment = prefab.SetEquipment(EquipmentType.Hand);
        equipment.WithQuickSlotType(QuickSlotType.Selectable);

        prefab.SetUnlock(ModItems.Ermfish);
        prefab.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);

        prefab.Register();

        ItemActionHandler.RegisterMiddleClickAction(prefab.Info.TechType, item => GymbagHandler.Instance.OnOpen(item), "open storage", "English");
    }

    private static Action<GameObject> ModifyPrefab(ItemData data)
    {
        return ModifyPrefabEncapsulated;

        void ModifyPrefabEncapsulated(GameObject prefab)
        {
            StorageContainer container = prefab.GetComponentInChildren<StorageContainer>();
            container.width = 4;
            container.height = 4;

            GameObject baseModel = prefab.GetComponentInChildren<Renderer>().gameObject;
            baseModel.SetActive(false);

            GameObject instance = Object.Instantiate(data.prefab, baseModel.transform.parent);

            PrefabUtils.AddVFXFabricating(instance, null, 0, 0.93f, new Vector3(0, -0.05f), 0.75f, Vector3.zero);

            MaterialUtils.ApplySNShaders(instance);
        }
    }
}
