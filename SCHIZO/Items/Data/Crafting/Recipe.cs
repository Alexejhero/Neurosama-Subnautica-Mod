using System.Linq;
using Nautilus.Crafting;

namespace SCHIZO.Items.Data.Crafting;

partial class Recipe
{
    private RecipeData _converted;

    public RecipeData Convert()
    {
        return _converted ??= new()
        {
            craftAmount = craftAmount,
            Ingredients = [..ingredients.Where(Ingredient.IsValid).Select(t => t.Convert())],
            LinkedItems = [..linkedItems.Where(Item.IsValid).Select(t => t.GetTechType())]
        };
    }
}
