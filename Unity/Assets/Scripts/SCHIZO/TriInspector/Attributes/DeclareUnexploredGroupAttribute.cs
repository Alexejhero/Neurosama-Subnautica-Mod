using TriInspector;

namespace SCHIZO.TriInspector.Attributes
{
    internal class DeclareUnexploredGroupAttribute : DeclareFoldoutGroupAttribute
    {
        public DeclareUnexploredGroupAttribute(string insideof) : base(insideof + "/unexplored")
        {
            base.Title = "Unexplored (These fields have not been explored yet and their functionality is unknown)";
        }

        public DeclareUnexploredGroupAttribute() : base("unexplored")
        {
            base.Title = "Unexplored (These fields have not been explored yet and their functionality is unknown)";
        }

        public new string Title => base.Title;
    }
}
