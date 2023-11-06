using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

[DeclareComponentReferencesGroup]
public class CreatureFlinch : MonoBehaviour
{
    [ComponentReferencesGroup, Required] public Animator animator;

    public float interval = 1;
    public float damageThreshold = 10;
}
