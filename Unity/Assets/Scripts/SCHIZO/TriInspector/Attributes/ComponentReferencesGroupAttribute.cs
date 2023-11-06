using TriInspector;

namespace SCHIZO.TriInspector.Attributes
{
    internal class ComponentReferencesGroupAttribute : GroupAttribute
    {
        public ComponentReferencesGroupAttribute() : base(DeclareComponentReferencesGroupAttribute.GROUP_NAME)
        {
        }
    }
}

// TODO: RequiredAutoFixGetComponent thing
