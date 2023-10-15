using NaughtyAttributes;
using SCHIZO.Utilities;
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

    [Foldout("Unchanged by Nautilus")] public bool controlModelState = true;
    [Foldout("Unchanged by Nautilus")] public MonoBehaviour[] controlledBehaviours;
    [Foldout("Unchanged by Nautilus")] public bool deconstructionAllowed = true;
    [Foldout("Unchanged by Nautilus")] public bool alignWithSurface = false;
    [Foldout("Unchanged by Nautilus")] public bool forceUpright = false;
    [Foldout("Unchanged by Nautilus")] public bool attachedToBase = false;
    [Foldout("Unchanged by Nautilus")] public float placeMaxDistance = 5;
    [Foldout("Unchanged by Nautilus")] public float placeMinDistance = 1.2f;
    [Foldout("Unchanged by Nautilus")] public float placeDefaultDistance = 2;
    [Foldout("Unchanged by Nautilus")] public GameObject builtBoxFX;

    [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public Object _techType;
    [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public Material ghostMaterial;
    [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public Texture _EmissiveTex;
    [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public Texture _NoiseTex;
}
