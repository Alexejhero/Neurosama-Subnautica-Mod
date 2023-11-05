using TriInspector;
using UnityEngine;

namespace TriExtensions
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

        private protected class UnexploredGroupAttribute : GroupAttribute
        {
            public UnexploredGroupAttribute(string insideof) : base(insideof + "/unexplored")
            {
            }

            public UnexploredGroupAttribute() : base("unexplored")
            {
            }
        }
    }
}
