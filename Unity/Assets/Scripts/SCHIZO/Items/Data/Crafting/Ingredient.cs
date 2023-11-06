using System;
using UnityEngine;

namespace SCHIZO.Items.Data.Crafting
{
    [Serializable]
    public sealed partial class Ingredient
    {
        [SerializeField]
        private Item item;

        [SerializeField]
        private int amount = 1;
    }
}
