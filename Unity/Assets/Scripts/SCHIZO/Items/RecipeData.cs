using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
{
    [Serializable]
    public sealed class RecipeData
    {
        public int craftAmount;
        public Ingredient[] ingredients;
        public global::TechType[] linkedItems;
    }
}
