using TriInspector;

namespace SCHIZO.TriInspector.Attributes
{
    internal class DeclareComponentReferencesGroupAttribute : DeclareFoldoutGroupAttribute
    {
        public const string GROUP_NAME = "component-references";

        public DeclareComponentReferencesGroupAttribute() : base(GROUP_NAME)
        {
            base.Title = "Component References";
        }

        public new string Title => base.Title;
    }
}
