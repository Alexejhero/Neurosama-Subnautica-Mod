using System.Collections.Generic;
using JetBrains.Annotations;
using SCHIZO.Options.Generic;
using SCHIZO.Registering;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Options
{
    [CreateAssetMenu(menuName = "SCHIZO/Options/Mod Option Panel")]
    public sealed partial class ModOptionPanel : ModRegistryItem
    {
        [SerializeField, ListDrawerSettings, UsedImplicitly]
        private List<ModOption> options;
    }
}
