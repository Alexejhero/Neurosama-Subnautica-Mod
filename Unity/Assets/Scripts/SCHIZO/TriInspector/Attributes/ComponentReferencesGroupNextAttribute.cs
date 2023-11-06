using TriInspector;

namespace SCHIZO.TriInspector.Attributes
{
    internal class ComponentReferencesGroupNextAttribute : GroupNextAttribute
    {
        public ComponentReferencesGroupNextAttribute() : base(DeclareComponentReferencesGroupAttribute.GROUP_NAME)
        {
        }
    }
}
