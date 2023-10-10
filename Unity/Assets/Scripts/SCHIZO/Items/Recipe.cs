using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
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
                Ingredients = new List<CraftData.Ingredient>(System.Linq.Enumerable.Select(ingredients, t => t.Convert())),
                LinkedItems = new List<TechType>(System.Linq.Enumerable.Select(linkedItems, t => t.Convert()))
            };
        }
#endif
    }
}
