using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Registering
{
    [CreateAssetMenu(menuName = "SCHIZO/Registering/Component Adder")]
    public sealed partial class ComponentAdder : ScriptableObject
    {
        [InfoBox("ComponentAdders are automatically registered.")]
        public bool isSingleton;

        [ValidateInput(nameof(ValidateTypeName)), HideIf(nameof(isSingleton))] public string typeName;
        [Dropdown(nameof(_methodNames)), HideIf(nameof(isSingleton))] public string methodName;

        [Required] public GameObject prefab;

        private bool ValidateTypeName(string val) => !string.IsNullOrWhiteSpace(val);
        private List<string> _methodNames = new List<string> {"Awake", "Start"};
    }
}
