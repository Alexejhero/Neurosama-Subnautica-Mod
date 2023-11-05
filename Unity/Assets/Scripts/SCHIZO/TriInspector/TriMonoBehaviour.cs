using TriInspector;
using UnityEngine;

namespace SCHIZO.TriInspector
{
    [DeclareFoldoutGroup("component-references", Title = "Component References")]
    [DeclareUnexploredGroup]
    public abstract class TriMonoBehaviour : MonoBehaviour
    {
        private protected class ComponentReferencesGroupAttribute : GroupAttribute
        {
            public ComponentReferencesGroupAttribute() : base("component-references")
            {
            }
        }

        private protected class ComponentReferencesGroupNextAttribute : GroupNextAttribute
        {
            public ComponentReferencesGroupNextAttribute() : base("component-references")
            {
            }
        }

        private protected class UnexploredGroupAttribute : GroupAttribute
        {
            public UnexploredGroupAttribute(string insideof) : base(insideof + "/unexplored")
            {
            }

            public UnexploredGroupAttribute() : base("unexplored")
            {
            }
        }

        private protected class UnexploredGroupNextAttribute : GroupNextAttribute
        {
            public UnexploredGroupNextAttribute(string insideof) : base(insideof + "/unexplored")
            {
            }

            public UnexploredGroupNextAttribute() : base("unexplored")
            {
            }
        }

        // TODO: RequiredAutoFixGetComponent thing
    }
}
