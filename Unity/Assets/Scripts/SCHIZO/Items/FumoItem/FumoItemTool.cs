using SCHIZO.Items.Components;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items.FumoItem
{
    [DeclareFoldoutGroup("Alt Effect")]
    public partial class FumoItemTool : CustomPlayerTool
    {
        [GroupNext("Alt Effect")]
        [Tooltip("Chance for the item to have the alternative use"), ShowIf(nameof(hasAltUse))]
        [Range(0,1), LabelText("Usable Chance")]
        public float altUsableChance = 0.05f;
        [Tooltip("If the item did not roll to be alt-usable, it has this chance to be able to trigger the alt-use effect on long hugs"), ShowIf(nameof(hasAltUse))]
        [Range(0,1), LabelText("Hug Chance")]
        public float altEffectOnHugChance = 0.5f;
        [Tooltip("Minimum hug duration to trigger alt-use effect"), ShowIf(nameof(hasAltUse))]
        [Range(0,60), LabelText("Min Hug Time")]
        public float altEffectMinHugTime = 10f;
        [Min(0), LabelText("Effect Duration"), ShowIf(nameof(hasAltUse))]
        public float altEffectDuration = 2f;

        [UnGroupNext]
        [Min(0)]
        public float hugCooldown = 1f;
        [Range(0, 100)]
        public int hugColdResistBuff = 20;
        [Required] public Transform fumoModelSN;
        [Required] public Transform fumoModelBZ;
    }
}
