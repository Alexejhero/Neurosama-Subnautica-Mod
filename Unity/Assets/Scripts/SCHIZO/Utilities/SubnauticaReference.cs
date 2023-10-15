using System;
using Object = UnityEngine.Object;

namespace SCHIZO.Utilities
{
    [Serializable]
    public class SubnauticaReference
    {
        public string typeName;
        public Object value;

        public SubnauticaReference(string typeName, Object defaultValue)
        {
            this.typeName = typeName;
            this.value = defaultValue;
        }
    }
}
