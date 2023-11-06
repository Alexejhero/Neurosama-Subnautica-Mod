using TriInspector;

namespace SCHIZO.TriInspector.Attributes
{
    internal class UnexploredGroupNextAttribute : GroupNextAttribute
    {
        public UnexploredGroupNextAttribute(string insideof) : base(insideof + "/unexplored")
        {
        }

        public UnexploredGroupNextAttribute() : base("unexplored")
        {
        }
    }
}
