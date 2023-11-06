using TriInspector;

namespace SCHIZO.TriInspector.Attributes
{
    internal class ComponentReferencesGroupAttribute : GroupAttribute
    {
        public const string GROUP_NAME = "component-references";

        public ComponentReferencesGroupAttribute() : base(GROUP_NAME)
        {
        }
    }
}

// TODO: RequiredAutoFixGetComponent thing
