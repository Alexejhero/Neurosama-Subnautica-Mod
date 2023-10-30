using System.Collections.Generic;

namespace SCHIZO.Registering
{
    partial class CompoundModRegistryItem
    {
        public HashSet<ModRegistryItem> Flatten(HashSet<ModRegistryItem> found = null)
        {
            if (found == null) found = new HashSet<ModRegistryItem>();
            if (found.Contains(this)) return found;
            found.Add(this);

            foreach (ModRegistryItem item in registryItems)
            {
                if (item && item is CompoundModRegistryItem registry)
                {
                    registry.Flatten(found);
                }
                found.Add(item);
            }

            return found;
        }
    }
}
