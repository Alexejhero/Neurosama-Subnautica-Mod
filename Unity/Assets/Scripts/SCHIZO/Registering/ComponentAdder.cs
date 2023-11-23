using System.Collections.Generic;
using JetBrains.Annotations;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Registering
{
    [CreateAssetMenu(menuName = "SCHIZO/Registering/Component Adder")]
    [DeclareBoxGroup("Target")]
    public sealed partial class ComponentAdder : ModRegistryItem
    {
        public enum Mode
        {
            Prefix,
            Postfix,
            CoroutineStep0Prefix,
        }

        [SerializeField, UsedImplicitly]
        private Game game;

        [SerializeField, Required, UsedImplicitly]
        private GameObject prefab;

        [GroupNext("Target")]

        [SerializeField]
        private bool isSingleton;

        [SerializeField, HideIf(nameof(isSingleton)), UsedImplicitly]
        [Tooltip("Used for targets that are already present in the scene by the time this is registered.\nCan be used to inject into e.g. main menu UI.")]
        private bool scanForExisting;

        [SerializeField, Required, HideIf(nameof(isSingleton)), UsedImplicitly]
        private string typeName;

        [SerializeField, Dropdown(nameof(_methodNames)), HideIf(nameof(isSingleton)), UsedImplicitly]
        private string methodName;

        [SerializeField, HideIf(nameof(isSingleton))]
        private bool _isBaseType;

        [SerializeField, ShowIf(nameof(TargetTypeNameShowIf)), UsedImplicitly]
        private string targetTypeName;

        [SerializeField, HideIf(nameof(isSingleton)), UsedImplicitly]
        private Mode mode = Mode.Postfix;

        #region NaughtyAttributes stuff

        private List<string> _methodNames = ["Awake", "Start"];
        private bool TargetTypeNameShowIf() => _isBaseType && !isSingleton;

        #endregion
    }
}
