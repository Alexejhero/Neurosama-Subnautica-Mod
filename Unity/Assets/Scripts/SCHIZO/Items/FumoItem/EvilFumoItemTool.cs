using TriInspector;
using UnityEngine;

namespace SCHIZO.Items.FumoItem
{
    public sealed partial class EvilFumoItemTool : FumoItemTool
    {
        [Group("Alt Effect")]
        [Range(0,100)]
        public float damageOnPoke;
        public bool stealKnife;
        public Transform knifeSocket;
    }
}
