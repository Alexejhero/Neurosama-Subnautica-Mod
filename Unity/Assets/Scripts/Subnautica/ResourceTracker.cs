using NaughtyAttributes;
using SCHIZO.Utilities;
using UnityEngine;

public class ResourceTracker : MonoBehaviour
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public Rigidbody rb;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public PrefabIdentifier prefabIdentifier;
    [Foldout(STRINGS.COMPONENT_REFERENCES)] public Pickupable pickupable;
}
