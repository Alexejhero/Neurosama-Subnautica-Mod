using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

public class Constructable : HandTarget
{
    [Required] public GameObject model;

    public bool allowedOnWall = false;
    public bool allowedOnGround = true;
    public bool allowedOnCeiling = false;
    public bool allowedInSub = true;
    public bool allowedInBase = true;
    public bool allowedOutside = false;
    public bool allowedOnConstructables = false;
    public bool allowedUnderwater = true;
    public bool rotationEnabled = false;
    public VFXSurfaceTypes surfaceType;

    [UnexploredGroup] public bool controlModelState = true;
    [UnexploredGroup] public MonoBehaviour[] controlledBehaviours;
    [UnexploredGroup] public bool deconstructionAllowed = true;
    [UnexploredGroup] public bool alignWithSurface = false;
    [UnexploredGroup] public bool forceUpright = false;
    [UnexploredGroup] public bool attachedToBase = false;
    [UnexploredGroup] public float placeMaxDistance = 5;
    [UnexploredGroup] public float placeMinDistance = 1.2f;
    [UnexploredGroup] public float placeDefaultDistance = 2;
    [UnexploredGroup] public GameObject builtBoxFX;

    // [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public TechType_All techType;
    // [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public Material ghostMaterial;
    // [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public Texture EmissiveTex;
    // [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public Texture NoiseTex;
}
