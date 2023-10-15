using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using SCHIZO.Utilities;
using UnityEngine;

namespace SCHIZO.Items
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Recipe")]
    public sealed class Recipe : ScriptableObject
    {
        [ValidateInput(nameof(game_Validate), "Game must be set!")]
        public Game game = Game.Subnautica | Game.BelowZero;
        private bool game_Validate(Game value) => value.HasFlag(Game.Subnautica) || value.HasFlag(Game.BelowZero);

        public int craftAmount = 1;
        [ReorderableList] public Ingredient[] ingredients;
        [ReorderableList] public Item[] linkedItems;

#if !UNITY
        public Nautilus.Crafting.RecipeData Convert()
        {
            return new Nautilus.Crafting.RecipeData
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
#endif
    }
}
