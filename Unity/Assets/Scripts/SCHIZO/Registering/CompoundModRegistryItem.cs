using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace SCHIZO.Registering
{
    [CreateAssetMenu(menuName = "SCHIZO/Registering/Compound Mod Registry Item")]
    public partial class CompoundModRegistryItem : ModRegistryItem
    {
        [FormerlySerializedAs("registered"), ReorderableList]
        public List<ModRegistryItem> registryItems = new List<ModRegistryItem>();
    }
}
