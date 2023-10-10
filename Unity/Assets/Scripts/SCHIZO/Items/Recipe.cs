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
        public Ingredient[] ingredients;
        public TechType_All[] linkedItems;
    }
}
