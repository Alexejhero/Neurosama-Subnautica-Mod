using System;

namespace SCHIZO.Items
{
    [Serializable]
    public sealed class Item
    {
        public bool isCustom = false;
        public TechType_All techType;
        public ItemData itemData;

#if !UNITY
        public TechType Convert()
        {
            if (!isCustom) return (TechType) (int) (object) techType;
            else return itemData.ModItem;
        }
#endif
    }
}
