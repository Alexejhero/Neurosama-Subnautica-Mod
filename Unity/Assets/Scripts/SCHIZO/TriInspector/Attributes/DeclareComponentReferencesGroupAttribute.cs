using TriInspector;

namespace SCHIZO.TriInspector.Attributes
{
    internal class DeclareComponentReferencesGroupAttribute : DeclareFoldoutGroupAttribute
    {
        public DeclareComponentReferencesGroupAttribute() : base(ComponentReferencesGroupAttribute.GROUP_NAME)
        {
            base.Title = "Component References";
        }

        public new string Title => base.Title;
    }
}
