using JetBrains.Annotations;
using SCHIZO.Interop.Subnautica;
using SCHIZO.Items.Data;
using UnityEngine;

namespace SCHIZO.Creatures.Components
{
    public partial class CustomCreatureTool : _CreatureTool
    {
        [SerializeField, UsedImplicitly] private CustomToolData data;
    }
}
