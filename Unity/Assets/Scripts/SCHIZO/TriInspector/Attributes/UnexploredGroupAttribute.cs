using TriInspector;

namespace SCHIZO.TriInspector.Attributes
{
    internal class UnexploredGroupAttribute : GroupAttribute
    {
        public UnexploredGroupAttribute(string insideof) : base(insideof + "/" + DeclareUnexploredGroupAttribute.GROUP_NAME)
        {
        }

        public UnexploredGroupAttribute() : base(DeclareUnexploredGroupAttribute.GROUP_NAME)
        {
        }
    }
}
