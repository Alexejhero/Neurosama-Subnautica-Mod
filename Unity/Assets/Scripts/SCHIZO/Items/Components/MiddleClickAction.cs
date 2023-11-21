using TriInspector;
using UnityEngine;

namespace SCHIZO.Items.Components
{
    public sealed partial class MiddleClickAction : MonoBehaviour
    {
        public string displayText;
        [Required]
        public MonoBehaviour target;
        [Required]
        public string method;
        [ListDrawerSettings]
        public string[] arguments; // yes this only supports primitives; no i don't care - Govo
    }
}
