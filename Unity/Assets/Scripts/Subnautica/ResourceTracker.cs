using SCHIZO.TriInspector;
using TriInspector;
using UnityEngine;

public class ResourceTracker : TriMonoBehaviour
{
    [ComponentReferencesGroup, Required] public Rigidbody rb;
    [ComponentReferencesGroup, Required] public PrefabIdentifier prefabIdentifier;
    [ComponentReferencesGroup] public Pickupable pickupable;
}
