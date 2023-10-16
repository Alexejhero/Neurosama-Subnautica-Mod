using System.Collections.Generic;
using JetBrains.Annotations;
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

        [SerializeField, ValidateInput(nameof(ValidateTypeName)), HideIf(nameof(isSingleton)), UsedImplicitly]
        private string typeName;

        [SerializeField, Dropdown(nameof(_methodNames)), HideIf(nameof(isSingleton)), UsedImplicitly]
        private string methodName;

        [SerializeField, HideIf(nameof(isSingleton))]
        private bool _isBaseType;

        [SerializeField, ShowIf(nameof(TargetTypeNameShowIf)), UsedImplicitly]
        private string targetTypeName;

        [SerializeField, HideIf(nameof(isSingleton)), UsedImplicitly]
        private Mode mode = Mode.Postfix;

        [SerializeField, Required, UsedImplicitly]
        private GameObject prefab;

        #region NaughtyAttributes stuff

        private bool ValidateTypeName(string val) => !string.IsNullOrWhiteSpace(val);
        private List<string> _methodNames = new List<string> {"Awake", "Start"};
        private bool TargetTypeNameShowIf() => _isBaseType && !isSingleton;

        #endregion
    }
}
