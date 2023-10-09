using Nautilus.Crafting;
using Nautilus.Handlers;
using SCHIZO.Attributes;
using SCHIZO.Resources;
using SCHIZO.Unity.Items;

namespace SCHIZO.BuildablesV2;

[LoadMethod]
public static class BuildablesLoader
{
    [LoadMethod]
    private static void Load()
    {
        ItemData testingData = Assets.NeurofumoV2_TESTINGDATA;

        ModItem item = new(testingData.classId, testingData.displayName, testingData.tooltip);
        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        item.PrefabInfo.WithIcon(testingData.icon).WithSizeInInventory(new Vector2int(testingData.itemSize.x, testingData.itemSize.y));

        new SealedBuildablePrefab(item, testingData).Register();

        CraftDataHandler.SetRecipeData(item, new RecipeData(new Ingredient(TechType.Titanium, 1)));
        CraftDataHandler.AddToGroup(TechGroup.Miscellaneous, TechCategory.Misc, item);
        CraftDataHandler.AddBuildable(item);
    }
}
