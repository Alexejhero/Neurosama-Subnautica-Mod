using System.Collections.Generic;
using System.Linq;
using Nautilus.Crafting;
using SCHIZO.Enums;

namespace SCHIZO.Items.Data.Crafting;

partial class Recipe
{
    public RecipeData Convert()
    {
        return new RecipeData
        {
            craftAmount = craftAmount,
            Ingredients = new List<NIngredient>(ingredients.Where(IngredientFilter).Select(t => t.Convert())),
            LinkedItems = new List<TechType>(linkedItems.Where(ItemFilter).Select(t => t.Convert()))
        };
    }

    private static bool IngredientFilter(Ingredient ingredient)
    {
        if (ingredient.amount <= 0) return false;
        return ItemFilter(ingredient.item);
    }

    private static bool ItemFilter(Item item)
    {
        if (item.isCustom && !item.itemData) return false;
        if (!item.isCustom && item.techType == TechType_All.None) return false;
        return true;
    }
}
