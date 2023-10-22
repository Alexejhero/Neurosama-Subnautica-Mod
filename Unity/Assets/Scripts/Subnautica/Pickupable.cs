using UnityEngine;

[DisallowMultipleComponent]
public class Pickupable : HandTarget
{
    public bool isPickupable = true;
    public bool destroyOnDeath = true;
    public bool randomizeRotationWhenDropped = true;
    [Tooltip("If this is true, isKinematic will be set to false when this is dropped, otherwise isKinematic will be set to true.")]
    public bool activateRigidbodyWhenDropped = true;
    public bool usePackUpIcon = false;
    // public ScaleLerp scaler;
    // [Tooltip("Optional Cinematic Controller that will be used to play a pickup animation instead of the generic Grab. Will NOT be used if the component is disabled (eg: maybe you only want this on the first pickup and disable it with a story goal).")] [SerializeField] [Publicized(1)]
    // public PlayerCinematicController pickupCinematic;
}
