using TriInspector;
using SCHIZO.Interop.Subnautica;
using SCHIZO.Items.Data;
using UnityEngine;

namespace SCHIZO.Creatures.Components
{
    [DeclareBoxGroup("ik_bz", Title = "IK Overrides (Below Zero)")]
    public partial class CustomCreatureTool : _CreatureTool
    {
        [SerializeField] private CustomToolData data;
    }
}
