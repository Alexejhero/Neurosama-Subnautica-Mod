using System;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
{
    [Serializable]
    public class Item<T> : IItem where T : Enum
    {
        public bool isBaseGame = true;
        public T techType;
        public ItemData itemData;

#if !UNITY
        public TechType Convert()
        {
            if (isBaseGame) return (TechType) (int) (object) techType;
            else return itemData.ModItem;
        }
#endif
    }

    public interface IItem
    {
#if !UNITY
        TechType Convert();
#endif
    }
}
