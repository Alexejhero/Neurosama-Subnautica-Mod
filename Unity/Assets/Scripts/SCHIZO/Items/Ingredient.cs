using System;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
{
    [Serializable]
    public class Ingredient<T> where T : IItem
    {
        public T item;
        public int amount = 1;

#if !UNITY
        public Ingredient Convert()
        {
            return new Ingredient(item.Convert(), amount);
        }
#endif
    }
}
