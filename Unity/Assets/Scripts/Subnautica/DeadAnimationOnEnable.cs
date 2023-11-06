using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

[DeclareComponentReferencesGroup]
public class DeadAnimationOnEnable : MonoBehaviour
{
    [ComponentReferencesGroup, Required] public LiveMixin liveMixin;

    [Required] public Animator animator;
    public bool disableAnimatorInstead;
}
