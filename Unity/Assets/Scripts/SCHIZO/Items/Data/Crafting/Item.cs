using System;
using JetBrains.Annotations;
using SCHIZO.Interop.Subnautica.Enums;
using UnityEngine;

namespace SCHIZO.Items.Data.Crafting
{
    [Serializable]
    public sealed partial class Item
    {
        [SerializeField, UsedImplicitly]
        private bool isCustom = false;

        [SerializeField, UsedImplicitly]
        private TechType_All techType;

        [SerializeField, UsedImplicitly]
        private ItemData itemData;
    }
}
