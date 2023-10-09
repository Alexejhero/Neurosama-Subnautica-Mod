using NaughtyAttributes;
using UnityEngine;

public class DeadAnimationOnEnable : MonoBehaviour
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public LiveMixin liveMixin;

    [Required] public Animator animator;
    public bool disableAnimatorInstead;
}
