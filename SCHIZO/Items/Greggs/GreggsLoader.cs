using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using SCHIZO.Attributes;
using SCHIZO.Resources;

namespace SCHIZO.Items.Greggs;

[LoadMethod]
public static class GreggsLoader
{
    [LoadMethod]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    private static void Load()
    {
        CustomPrefab deadErmfish = new(PrefabInfo.WithTechType("deadermfish", "Dead Ermfish", "erm\n<size=75%>(Model by w1n7er)</size>"));
        deadErmfish.Info.WithIcon(ResourceManager.LoadAsset<Sprite>("dead erm"));
        deadErmfish.Register();

        CustomPrefab greggs = new(PrefabInfo.WithTechType("greggs", "Greggs", "god i fucking love greggs i would sell neuro just to lick a chicken bake right now holy shit i would marry gregg where is he"));
        greggs.Info.WithIcon(ResourceManager.LoadAsset<Sprite>("greggs"));

        CraftingGadget crafting = greggs.SetRecipe(new RecipeData(new Ingredient(deadErmfish.Info.TechType, 1)));
        crafting.WithFabricatorType(CraftTree.Type.Fabricator);
        crafting.WithStepsToFabricatorTab(CraftTreeHandler.Paths.FabricatorCookedFood);

        greggs.SetUnlock(ModItems.Ermfish);
        greggs.SetPdaGroupCategory(TechGroup.Survival, Retargeting.TechCategory.CookedFood);

        greggs.Register();
    }
}
