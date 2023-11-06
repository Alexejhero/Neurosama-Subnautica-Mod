using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

[DeclareComponentReferencesGroup]
public class AnimateByVelocity : MonoBehaviour
{
    [ComponentReferencesGroup, Required] public BehaviourLOD levelOfDetail;
    [ComponentReferencesGroup, Required] public Animator animator;

    [Required, SceneObjectsOnly] public GameObject rootGameObject;
    public float animationMoveMaxSpeed = 4;
    public float animationMaxPitch = 30;
    public float animationMaxTilt = 45;
    public bool useStrafeAnimation = false;
    public float dampTime = 0.5f;

    // ReSharper disable once Unity.RedundantEventFunction
    private void OnEnable() {}
}
