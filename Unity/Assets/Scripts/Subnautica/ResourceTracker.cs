using NaughtyAttributes;
using UnityEngine;

public class ResourceTracker : MonoBehaviour
{
    [Foldout(STRINGS.COMPONENT_REFERENCES)] public Rigidbody rb;
    // [Tooltip("Optional")] public Pickupable pickupable; TODO

    [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public Object _prefabIdentifier;
}
