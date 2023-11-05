using TriInspector;
using UnityEngine;

namespace SCHIZO.TriInspector
{
    public abstract class TriScriptableObject : ScriptableObject
    {
        private protected class UnexploredGroupAttribute : GroupAttribute
        {
            public UnexploredGroupAttribute(string insideof) : base(insideof + "/unexplored")
            {
            }

            public UnexploredGroupAttribute() : base("unexplored")
            {
            }
        }

        private protected class UnexploredGroupNextAttribute : GroupNextAttribute
        {
            public UnexploredGroupNextAttribute(string insideof) : base(insideof + "/unexplored")
            {
            }

            public UnexploredGroupNextAttribute() : base("unexplored")
            {
            }
        }
    }
}
