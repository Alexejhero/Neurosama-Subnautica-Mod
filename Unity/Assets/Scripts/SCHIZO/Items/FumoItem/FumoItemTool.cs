using SCHIZO.Items.Components;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items.FumoItem
{
    public sealed partial class FumoItemTool : CustomPlayerTool
    {
        public float hugCooldown = 1f;
        [Range(0, 100)]
        public int hugColdResistBuff = 20;
        [Required] public Transform fumoModel;
    }
}
