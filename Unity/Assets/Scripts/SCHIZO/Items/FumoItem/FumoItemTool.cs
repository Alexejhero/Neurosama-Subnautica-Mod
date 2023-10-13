using UnityEngine;

namespace SCHIZO.Unity.Items.FumoItem
{
    public sealed partial class FumoItemTool : CustomPlayerTool
    {
        public float hugCooldown = 1f;
        [Range(0, 100)]
        public int hugColdResistBuff = 20;
        public Transform fumoModel;
    }
}
