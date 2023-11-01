using NaughtyAttributes;
using SCHIZO.Utilities;
using UnityEngine;

public class LandCreatureGravity : MonoBehaviour
{
    public float downforce = 10;
    public float aboveWaterGravity = 9.81f;
    public float underWaterGravity = 2.7f;
    public bool applyDownforceUnderwater;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool canGoInStasisUnderwater = true;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public OnSurfaceTracker onSurfaceTracker;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public LiveMixin liveMixin;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public Rigidbody creatureRigidbody;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public WorldForces worldForces;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public SphereCollider bodyCollider;
    [Foldout(STRINGS.COMPONENT_REFERENCES)] public Pickupable pickupable;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool trackSurfaceCollider = true;
    // public LandCreatureGravity.OnInStasisStateChange onInStasisStateChange;
}
