using JetBrains.Annotations;
using NaughtyAttributes;
using SCHIZO.Enums;
using UnityEngine;

namespace SCHIZO.Items.Data
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Clone Item Data")]
    public sealed partial class CloneItemData : ItemData
    {
        [BoxGroup("Common Properties"), ReadOnly, ValidateInput(nameof(loader_Validate))]
        public ItemLoader loader;

        [BoxGroup("Subnautica Data"), Label("Clone Target"), SerializeField, UsedImplicitly]
        private TechType_All cloneTargetSN;

        [BoxGroup("Below Zero Data"), Label("Clone Target"), SerializeField, UsedImplicitly]
        private TechType_All cloneTargetBZ;

        #region NaughtyAttributes stuff

        private bool loader_Validate(ItemLoader val) => !autoRegister || val;

        #endregion
    }
}
