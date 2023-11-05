using SCHIZO.Attributes;
using SCHIZO.TriInspector;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    partial class _LandCreatureGravity : TriMonoBehaviour
    {
        [ComponentReferencesGroup, Required, ExposedType("OnSurfaceTracker")] public MonoBehaviour onSurfaceTracker;
        [ComponentReferencesGroup, Required, ExposedType("LiveMixin")] public MonoBehaviour liveMixin;
        [ComponentReferencesGroup, Required] public Rigidbody creatureRigidbody;
        [ComponentReferencesGroup, Required, ExposedType("WorldForces")] public MonoBehaviour worldForces;
        [ComponentReferencesGroup, Required] public SphereCollider bodyCollider;
        [ComponentReferencesGroup, ExposedType("Pickupable")] public MonoBehaviour pickupable;

        public float downforce = 10;
        public float aboveWaterGravity = 9.81f;
        public float underWaterGravity = 2.7f;
        public bool applyDownforceUnderwater;

        [UnexploredGroup] public bool canGoInStasisUnderwater = true;
        [UnexploredGroup] public bool trackSurfaceCollider = true;
        // public LandCreatureGravity.OnInStasisStateChange onInStasisStateChange;
    }
}
