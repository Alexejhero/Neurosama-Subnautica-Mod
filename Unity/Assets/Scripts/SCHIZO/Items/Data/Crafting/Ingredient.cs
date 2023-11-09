using System;
using JetBrains.Annotations;
using UnityEngine;

namespace SCHIZO.Items.Data.Crafting
{
    [Serializable]
    public sealed partial class Ingredient
    {
        [SerializeField, UsedImplicitly]
        private Item item;

        [SerializeField, UsedImplicitly]
        private int amount = 1;
    }
}
