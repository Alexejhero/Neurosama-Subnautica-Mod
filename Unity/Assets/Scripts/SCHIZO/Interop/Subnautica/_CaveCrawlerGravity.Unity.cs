using SCHIZO.Attributes;
using SCHIZO.TriInspector;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    partial class _CaveCrawlerGravity : TriMonoBehaviour
    {
        [ComponentReferencesGroup, Required] public _CaveCrawler caveCrawler;
        [ComponentReferencesGroup, Required, ExposedType("LiveMixin")] public MonoBehaviour liveMixin;
        [ComponentReferencesGroup, Required] public Rigidbody crawlerRigidbody;
    }
}
