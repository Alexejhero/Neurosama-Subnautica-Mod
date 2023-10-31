using System.Collections.Generic;
using NaughtyAttributes;
using SCHIZO.Registering;
using UnityEngine;

namespace SCHIZO.Options
{
    [CreateAssetMenu(menuName = "SCHIZO/Options/Mod Option Panel")]
    public sealed partial class ModOptionPanel : ModRegistryItem
    {
        [SerializeField, ReorderableList]
        private List<ModOption> options;
    }
}
