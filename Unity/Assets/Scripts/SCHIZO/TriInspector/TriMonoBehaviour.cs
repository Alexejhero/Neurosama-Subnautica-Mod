using TriInspector;
using UnityEngine;

namespace SCHIZO.TriInspector
{
    [DeclareFoldoutGroup("component-references", Title = "Component References")]
    public abstract class TriMonoBehaviour : MonoBehaviour
    {
        protected internal class ComponentReferencesGroupAttribute : GroupAttribute
        {
            public ComponentReferencesGroupAttribute() : base("component-references")
            {
            }
        }
    }
}
