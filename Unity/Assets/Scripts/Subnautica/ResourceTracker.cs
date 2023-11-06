using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

[DeclareComponentReferencesGroup]
[DeclareUnexploredGroup]
public class ResourceTracker : MonoBehaviour
{
    [ComponentReferencesGroup, Required] public Rigidbody rb;
    [ComponentReferencesGroup, Required] public PrefabIdentifier prefabIdentifier;
    [ComponentReferencesGroup] public Pickupable pickupable;
}
