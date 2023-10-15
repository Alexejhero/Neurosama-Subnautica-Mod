using NaughtyAttributes;
using SCHIZO.Utilities;
using UnityEngine;

public class AnimateByVelocity : MonoBehaviour
{
    [Required] public GameObject rootGameObject;
    [Required] public Animator animator;

    public float animationMoveMaxSpeed = 4;
    public float animationMaxPitch = 30;
    public float animationMaxTilt = 45;
    public bool useStrafeAnimation = false;
    public float dampTime = 0.5f;

    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public BehaviourLOD levelOfDetail;
}
