using NaughtyAttributes;
using SCHIZO.Interop.Subnautica;
using SCHIZO.Items.Data;
using UnityEngine;

namespace SCHIZO.Items
{
    public partial class CustomPlayerTool : _PlayerTool
    {
        [SerializeField] private CustomToolData data;

        [Foldout("Use Text"), SerializeField]
        protected bool hasPrimaryUse;
        [Foldout("Use Text"), SerializeField, ShowIf(nameof(hasPrimaryUse))]
        protected string primaryUseText;
        [Foldout("Use Text"), SerializeField]
        protected bool hasSecondaryUse;
        [Foldout("Use Text"), SerializeField, ShowIf(nameof(hasSecondaryUse))]
        protected string secondaryUseText;
        [Foldout("Use Text"), SerializeField]
        protected bool hasAltUse;
        [Foldout("Use Text"), SerializeField, ShowIf(nameof(hasAltUse))]
        protected string altUseText;
    }
}
