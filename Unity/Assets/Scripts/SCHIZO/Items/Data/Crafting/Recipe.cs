using SCHIZO.Registering;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items.Data.Crafting
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Recipe")]
    public sealed partial class Recipe : ScriptableObject
    {
        [OnValueChanged("OnGameChanged")]
        public Game game = Game.Subnautica | Game.BelowZero;

        public int craftAmount = 1;
        [ListDrawerSettings] public Ingredient[] ingredients;
        [ListDrawerSettings] public Item[] linkedItems;
    }
}
