using System;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
{
    [Serializable]
    public sealed class Item
    {
        public bool isBaseGame = true;
        public TechType_All techType;
        public ItemData itemData;

#if !UNITY
        public TechType Convert()
        {
            if (isBaseGame) return (TechType) (int) (object) techType;
            else return itemData.ModItem;
        }
#endif
    }
}
