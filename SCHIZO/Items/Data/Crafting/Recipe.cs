using System.Collections.Generic;
using System.Linq;
using Nautilus.Crafting;
using SCHIZO.Interop.Subnautica.Enums;

namespace SCHIZO.Items.Data.Crafting;

partial class Recipe
{
    private RecipeData _converted;

    public RecipeData Convert()
    {
        if (_converted != null) return _converted;

        return _converted = new RecipeData
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
