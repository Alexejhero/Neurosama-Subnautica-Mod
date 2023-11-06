using JetBrains.Annotations;
using SCHIZO.Interop.Subnautica;
using SCHIZO.Items.Data;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items.Components
{
    [DeclareFoldoutGroup("Use Text")]
    public partial class CustomPlayerTool : _PlayerTool
    {
        [SerializeField, UsedImplicitly] private CustomToolData data;

        [GroupNext("Use Text"), SerializeField]
        protected bool hasPrimaryUse;
        [SerializeField, ShowIf(nameof(hasPrimaryUse))]
        protected string primaryUseText;
        [SerializeField]
        protected bool hasSecondaryUse;
        [SerializeField, ShowIf(nameof(hasSecondaryUse))]
        protected string secondaryUseText;
        [SerializeField]
        protected bool hasAltUse;
        [SerializeField, ShowIf(nameof(hasAltUse))]
        protected string altUseText;
    }
}
