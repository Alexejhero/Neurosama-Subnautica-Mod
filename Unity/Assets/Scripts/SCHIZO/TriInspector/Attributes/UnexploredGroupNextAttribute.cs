using TriInspector;

namespace SCHIZO.TriInspector.Attributes
{
    internal class UnexploredGroupNextAttribute : GroupNextAttribute
    {
        public UnexploredGroupNextAttribute(string insideof) : base(insideof + "/" + DeclareUnexploredGroupAttribute.GROUP_NAME)
        {
        }

        public UnexploredGroupNextAttribute() : base(DeclareUnexploredGroupAttribute.GROUP_NAME)
        {
        }
    }
}
