using JetBrains.Annotations;
using NaughtyAttributes;
using SCHIZO.Interop.Subnautica.Enums;
using UnityEngine;

namespace SCHIZO.Items.Data
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Clone Item Data")]
    public sealed partial class CloneItemData : ItemData
    {
        [BoxGroup("Common Properties"), ReadOnly, Required]
        public CloneItemLoader loader;

        [BoxGroup("Subnautica Data"), Label("Clone Target"), SerializeField, UsedImplicitly, ShowIf(nameof(registerInSN))]
        private TechType_All cloneTargetSN;

        [BoxGroup("Below Zero Data"), Label("Clone Target"), SerializeField, UsedImplicitly, ShowIf(nameof(registerInBZ))]
        private TechType_All cloneTargetBZ;
    }
}
