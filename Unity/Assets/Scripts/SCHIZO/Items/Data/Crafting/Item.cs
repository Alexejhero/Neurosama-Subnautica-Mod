using System;
using SCHIZO.Interop.Subnautica.Enums;
using UnityEngine;

namespace SCHIZO.Items.Data.Crafting
{
    [Serializable]
    public sealed partial class Item
    {
        [SerializeField]
        private bool isCustom = false;

        [SerializeField]
        private TechType_All techType;

        [SerializeField]
        private ItemData itemData;
    }
}
