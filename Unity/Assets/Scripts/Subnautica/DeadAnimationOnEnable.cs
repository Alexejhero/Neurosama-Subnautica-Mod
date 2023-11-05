using SCHIZO.TriInspector;
using TriInspector;
using UnityEngine;

public class DeadAnimationOnEnable : TriMonoBehaviour
{
    [ComponentReferencesGroup, Required] public LiveMixin liveMixin;

    [Required] public Animator animator;
    public bool disableAnimatorInstead;
}
