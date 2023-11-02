using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using SCHIZO.Utilities;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    partial class _CaveCrawlerGravity : MonoBehaviour
    {
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public _CaveCrawler caveCrawler;
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required, ExposedType("LiveMixin")] public MonoBehaviour liveMixin;
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public Rigidbody crawlerRigidbody;
    }
}
