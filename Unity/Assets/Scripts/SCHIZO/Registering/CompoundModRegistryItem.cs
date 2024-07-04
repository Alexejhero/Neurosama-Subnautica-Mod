using System.Collections.Generic;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace SCHIZO.Registering
{
    [CreateAssetMenu(menuName = "SCHIZO/Registering/Compound Mod Registry Item")]
    public partial class CompoundModRegistryItem : ModRegistryItem
    {
        [FormerlySerializedAs("registered"), ListDrawerSettings(AlwaysExpanded = true)]
        public List<ModRegistryItem> registryItems = [];
    }
}
