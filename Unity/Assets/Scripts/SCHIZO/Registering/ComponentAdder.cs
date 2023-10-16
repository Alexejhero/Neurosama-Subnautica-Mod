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

        [SerializeField]
        private bool isSingleton;

        [SerializeField, ValidateInput(nameof(ValidateTypeName)), HideIf(nameof(isSingleton))]
        private string typeName;

        [SerializeField, Dropdown(nameof(_methodNames)), HideIf(nameof(isSingleton))]
        private string methodName;

        [SerializeField, HideIf(nameof(isSingleton))]
        private bool _isBaseType;

        [SerializeField, ShowIf(nameof(TargetTypeNameShowIf))]
        private string targetTypeName;

        [SerializeField, HideIf(nameof(isSingleton))]
        private Mode mode = Mode.Postfix;

        [SerializeField, Required]
        private GameObject prefab;

        #region NaughtyAttributes stuff

        private bool ValidateTypeName(string val) => !string.IsNullOrWhiteSpace(val);
        private List<string> _methodNames = new List<string> {"Awake", "Start"};
        private bool TargetTypeNameShowIf() => _isBaseType && !isSingleton;

        #endregion
    }
}
