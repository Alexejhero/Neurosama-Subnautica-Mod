using System;
using SCHIZO.Interop.Subnautica.Enums;

namespace SCHIZO.Items.Data.Crafting
{
    [Serializable]
    public sealed partial class Item
    {
        public bool isCustom = false;
        public TechType_All techType;
        public ItemData itemData;
    }
}
