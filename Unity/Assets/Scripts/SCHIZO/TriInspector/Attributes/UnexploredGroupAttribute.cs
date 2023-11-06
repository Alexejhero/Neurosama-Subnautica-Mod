using TriInspector;

namespace SCHIZO.TriInspector.Attributes
{
    internal class UnexploredGroupAttribute : GroupAttribute
    {
        public UnexploredGroupAttribute(string insideof) : base(insideof + "/unexplored")
        {
        }

        public UnexploredGroupAttribute() : base("unexplored")
        {
        }
    }
}
