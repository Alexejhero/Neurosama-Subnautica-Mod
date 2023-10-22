using NaughtyAttributes;
using SCHIZO.Registering;
using UnityEngine;

namespace SCHIZO.Items.Data.Crafting
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Recipe")]
    public sealed partial class Recipe : ScriptableObject
    {
        public Game game = Game.Subnautica | Game.BelowZero;

        public int craftAmount = 1;
        [ReorderableList] public Ingredient[] ingredients;
        [ReorderableList] public Item[] linkedItems;
    }
}
