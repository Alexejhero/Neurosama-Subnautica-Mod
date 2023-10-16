using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Registering
{
    [CreateAssetMenu(menuName = "SCHIZO/Registering/Component Adder")]
    public sealed partial class ComponentAdder : ModRegistryItem
    {
        public enum Mode
        {
            Prefix,
            Postfix,
            CoroutineStep0Prefix,
        }

        public bool isSingleton;
        [ValidateInput(nameof(ValidateTypeName)), HideIf(nameof(isSingleton))] public string typeName;
        [Dropdown(nameof(_methodNames)), HideIf(nameof(isSingleton))] public string methodName;
        [HideIf(nameof(isSingleton)), SerializeField] private bool _isBaseType = false;
        [ShowIf(nameof(TargetTypeNameShowIf))] public string targetTypeName;
        [HideIf(nameof(isSingleton))] public Mode mode = Mode.Postfix;

        [Required] public GameObject prefab;

        private bool ValidateTypeName(string val) => !string.IsNullOrWhiteSpace(val);
        private List<string> _methodNames = new List<string> {"Awake", "Start"};
        private bool TargetTypeNameShowIf() => _isBaseType && !isSingleton;
    }
}
