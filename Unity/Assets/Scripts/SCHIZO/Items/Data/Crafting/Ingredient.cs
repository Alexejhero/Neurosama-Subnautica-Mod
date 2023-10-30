using System;

namespace SCHIZO.Items.Data.Crafting
{
    [Serializable]
    public sealed partial class Ingredient
    {
        public Item item;
        public int amount = 1;
    }
}
