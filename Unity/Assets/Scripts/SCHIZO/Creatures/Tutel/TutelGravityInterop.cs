using SCHIZO.Attributes;
using SCHIZO.Interop.Subnautica;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Creatures.Tutel
{
    [DeclareBoxGroup("sn", Title = "Subnautica")]
    [DeclareBoxGroup("bz", Title = "Below Zero")]
    public sealed partial class TutelGravityInterop : MonoBehaviour
    {
        [Required] public Rigidbody creatureRigidbody;
        [Required, ExposedType("LiveMixin")] public MonoBehaviour liveMixin;
        [Group("sn")]
        [Required] public _CaveCrawler caveCrawler;

        [GroupNext("bz")]
        [Required, ExposedType("OnSurfaceTracker")] public MonoBehaviour onSurfaceTracker;
        [Required, ExposedType("WorldForces")] public MonoBehaviour worldForces;
        [Required] public SphereCollider bodyCollider;
        [ExposedType("Pickupable")] public MonoBehaviour pickupable;

        public float downforce = 10;
        public float aboveWaterGravity = 9.81f;
        public float underWaterGravity = 2.7f;
        public bool applyDownforceUnderwater;

        public bool canGoInStasisUnderwater = true;
        public bool trackSurfaceCollider = true;
    }
}
