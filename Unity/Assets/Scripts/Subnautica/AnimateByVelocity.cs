using NaughtyAttributes;
using UnityEngine;

public class AnimateByVelocity : MonoBehaviour
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public Animator animator;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public GameObject rootGameObject;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public BehaviourLOD levelOfDetail;

    public float animationMoveMaxSpeed = 4;
    public float animationMaxPitch = 30;
    public float animationMaxTilt = 45;
    public bool useStrafeAnimation = false;
    public float dampTime = 0.5f;
}
