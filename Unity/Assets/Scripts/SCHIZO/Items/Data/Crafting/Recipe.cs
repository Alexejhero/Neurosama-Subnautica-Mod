using NaughtyAttributes;
using SCHIZO.Enums;
using UnityEngine;

namespace SCHIZO.Items.Data.Crafting
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Recipe")]
    public sealed partial class Recipe : ScriptableObject
    {
        [ValidateInput(nameof(game_Validate), "Game must be set!")]
        public Game game = Game.Subnautica | Game.BelowZero;
        private bool game_Validate(Game value) => value.HasFlag(Game.Subnautica) || value.HasFlag(Game.BelowZero);

        public int craftAmount = 1;
        [ReorderableList] public Ingredient[] ingredients;
        [ReorderableList] public Item[] linkedItems;
    }
}
