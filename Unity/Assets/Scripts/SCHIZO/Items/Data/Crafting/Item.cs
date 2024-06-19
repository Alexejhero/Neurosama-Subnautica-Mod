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
        private bool isCustom;

        [SerializeField, UsedImplicitly]
        private TechType_All techType;

        [SerializeField, UsedImplicitly]
        private ItemData itemData;

        public bool IsCustom => isCustom;
        public TechType_All TechType => isCustom ? default : techType;
        public ItemData ItemData => isCustom ? itemData : null;
    }
}
