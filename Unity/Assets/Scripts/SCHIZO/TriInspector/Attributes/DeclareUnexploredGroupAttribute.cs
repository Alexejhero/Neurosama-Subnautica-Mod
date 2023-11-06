using TriInspector;

namespace SCHIZO.TriInspector.Attributes
{
    internal class DeclareUnexploredGroupAttribute : DeclareFoldoutGroupAttribute
    {
        public const string GROUP_NAME = "unexplored";

        public DeclareUnexploredGroupAttribute(string insideof) : base(insideof + "/" + GROUP_NAME)
        {
            base.Title = "Unexplored (These fields have not been explored yet and their functionality is unknown)";
        }

        public DeclareUnexploredGroupAttribute() : base(GROUP_NAME)
        {
            base.Title = "Unexplored (These fields have not been explored yet and their functionality is unknown)";
        }

        public new string Title => base.Title;
    }
}
