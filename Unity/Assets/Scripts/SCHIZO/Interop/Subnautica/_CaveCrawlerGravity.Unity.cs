using SCHIZO.Attributes;
using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    [DeclareComponentReferencesGroup]
    partial class _CaveCrawlerGravity : MonoBehaviour
    {
        [ComponentReferencesGroup, Required] public _CaveCrawler caveCrawler;
        [ComponentReferencesGroup, Required, ExposedType("LiveMixin")] public MonoBehaviour liveMixin;
        [ComponentReferencesGroup, Required] public Rigidbody crawlerRigidbody;
    }
}
