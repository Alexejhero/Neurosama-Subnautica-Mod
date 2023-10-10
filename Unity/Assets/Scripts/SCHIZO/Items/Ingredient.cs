using System;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
{
    [Serializable]
    public sealed class Ingredient
    {
        public Item item;
        public int amount = 1;

#if !UNITY
        public NIngredient Convert()
        {
            return new NIngredient(item.Convert(), amount);
        }
#endif
    }
}
