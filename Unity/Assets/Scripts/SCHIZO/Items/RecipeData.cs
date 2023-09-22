using System;
using NaughtyAttributes;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
{
    [Serializable]
    public sealed class RecipeData
    {
        public int craftAmount = 1;
        public Ingredient[] ingredients;
        public Item[] linkedItems;
    }
}
