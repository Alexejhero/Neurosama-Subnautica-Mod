using NaughtyAttributes;
using UnityEngine;

public class ResourceTracker : MonoBehaviour
{
    [Foldout(STRINGS.COMPONENT_REFERENCES)] public Rigidbody rb;
    [Foldout(STRINGS.COMPONENT_REFERENCES)] public PrefabIdentifier prefabIdentifier;
    // [Tooltip("Optional")] public Pickupable pickupable; TODO
}
