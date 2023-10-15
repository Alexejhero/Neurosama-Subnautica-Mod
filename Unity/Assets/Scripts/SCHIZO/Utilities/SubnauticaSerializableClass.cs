using System;
using System.Collections.Generic;

namespace SCHIZO.Utilities
{
    [Serializable]
    public class SubnauticaSerializableClass
    {
        public string typeName;
        public Dictionary<string, object> values;

        public SubnauticaSerializableClass(string typeName, Dictionary<string, object> values = null)
        {
            this.typeName = typeName;
            this.values = values ?? new Dictionary<string, object>();
        }
    }
}
