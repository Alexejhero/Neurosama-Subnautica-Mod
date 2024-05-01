using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

[DeclareComponentReferencesGroup]
public class DestroyDuplicates : MonoBehaviour
{
    [ComponentReferencesGroup, Required] public PrefabIdentifier identifier;
}
