using NaughtyAttributes;
using SCHIZO.Interop.Subnautica;
using SCHIZO.Utilities;
using UnityEngine;

public class CaveCrawlerGravity : MonoBehaviour
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public _CaveCrawler caveCrawler;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public LiveMixin liveMixin;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public Rigidbody crawlerRigidbody;
}
