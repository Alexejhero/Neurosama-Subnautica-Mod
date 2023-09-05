using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;

namespace SCHIZO.Items.Greggs;

public static class GreggsLoader
{
    public static void Load()
    {
        CustomPrefab deadErmfish = new(PrefabInfo.WithTechType("deadermfish", "Dead Ermfish", "erm\n<size=75%>(Model by w1n7er)</size>"));
        deadErmfish.Info.WithIcon(AssetLoader.GetAtlasSprite("erm_dead.png"));
        deadErmfish.Register();

        CustomPrefab greggs = new(PrefabInfo.WithTechType("greggs", "Greggs", "god i fucking love greggs i would sell neuro just to lick a chicken bake right now holy shit i would marry gregg where is he"));
        greggs.Info.WithIcon(AssetLoader.GetAtlasSprite("greggs.png"));

        CraftingGadget crafting = greggs.SetRecipe(new RecipeData(new CraftData.Ingredient(deadErmfish.Info.TechType)));
        crafting.WithFabricatorType(CraftTree.Type.Fabricator);
        crafting.WithStepsToFabricatorTab(CraftTreeHandler.Paths.FabricatorCookedFood);

        greggs.SetUnlock(ModItems.Ermfish);
        greggs.SetPdaGroupCategory(TechGroup.Survival, TechCategory.CookedFood);

        greggs.Register();
    }
}
