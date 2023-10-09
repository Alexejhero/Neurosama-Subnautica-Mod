using NaughtyAttributes;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
{
    public sealed class Recipe : ScriptableObject
    {
        [ValidateInput(nameof(game_Validate), "Game must be set!")]
        public Game game;
        private bool game_Validate(Game value) => value.HasFlag(Game.Subnautica) || value.HasFlag(Game.BelowZero);

        public int craftAmount = 1;

        [SerializeField, ReorderableList, ShowIf()] private Ingredient_SN[] ingredientsSN;
        [SerializeField, ReorderableList, ShowIf()] private Ingredient_BZ[] ingredientsSN;
    }
}
