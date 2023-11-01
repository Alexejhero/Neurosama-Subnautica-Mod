using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using SCHIZO.Utilities;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    partial class _LandCreatureGravity : MonoBehaviour
    {
        public float downforce = 10;
        public float aboveWaterGravity = 9.81f;
        public float underWaterGravity = 2.7f;
        public bool applyDownforceUnderwater;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool canGoInStasisUnderwater = true;
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required, ExposedType("OnSurfaceTracker")] public MonoBehaviour onSurfaceTracker;
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required, ExposedType("LiveMixin")] public MonoBehaviour liveMixin;
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public Rigidbody creatureRigidbody;
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required, ExposedType("WorldForces")] public MonoBehaviour worldForces;
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public SphereCollider bodyCollider;
        [Foldout(STRINGS.COMPONENT_REFERENCES), ExposedType("Pickupable")] public MonoBehaviour pickupable;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool trackSurfaceCollider = true;
        // public LandCreatureGravity.OnInStasisStateChange onInStasisStateChange;
    }
}
