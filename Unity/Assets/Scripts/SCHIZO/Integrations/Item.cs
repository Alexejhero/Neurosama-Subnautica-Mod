using System;
using NaughtyAttributes;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
{
    [Serializable]
    public sealed class Item
    {
        public bool isCustom;

        private bool showIndividualTechTypes => !isCustom && isDifferent;
        private bool showUniqueTechType => !isCustom && !isDifferent;

        [AllowNesting, HideIf(nameof(isCustom))]
        public bool isDifferent;

        [AllowNesting, ShowIf(nameof(showUniqueTechType))]
        public TechType_SN techType;

        [AllowNesting, ShowIf(nameof(showIndividualTechTypes))]
        [Label("Tech Type (SN)")]
        public TechType_SN techTypeSN;

        [AllowNesting, ShowIf(nameof(showIndividualTechTypes))]
        [Label("Tech Type (BZ)")]
        public TechType_BZ techTypeBZ;

        [AllowNesting]
        [ShowIf(nameof(isCustom))]
        public ItemData itemData;
    }
}
