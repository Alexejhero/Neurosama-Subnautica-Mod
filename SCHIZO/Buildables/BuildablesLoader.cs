using Nautilus.Crafting;
using Nautilus.Handlers;
using SCHIZO.Attributes;
using SCHIZO.Items;
using SCHIZO.Resources;
using SCHIZO.Unity.Items;

namespace SCHIZO.Buildables;

[LoadMethod]
public static class BuildablesLoader
{
    [LoadMethod]
    private static void Load()
    {
        ModItem item = new(Assets.NeurofumoV2_TESTINGDATA);
        new SealedBuildablePrefab(item).Register();
        CraftDataHandler.SetRecipeData(item, new RecipeData(new Ingredient(TechType.Titanium, 1)));
    }
}
