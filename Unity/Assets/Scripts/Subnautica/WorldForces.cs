﻿using NaughtyAttributes;
using UnityEngine;

public class WorldForces : MonoBehaviour
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public Rigidbody useRigidbody;

    public bool moveWithPlatform;

    [BoxGroup("Gravity")] public bool handleGravity = true;
    [BoxGroup("Gravity"), ShowIf(nameof(handleGravity))] public float aboveWaterGravity = 9.81f;
    [BoxGroup("Gravity"), ShowIf(nameof(handleGravity))] public float underwaterGravity = 0.0f;

    [BoxGroup("Drag")] public bool handleDrag = false; // Base-game default: true
    [BoxGroup("Drag"), ShowIf(nameof(handleDrag))] public float aboveWaterDrag = 0.0f; // Base-game default: 0.1f
    [BoxGroup("Drag"), ShowIf(nameof(handleDrag))] public float underwaterDrag = 0.1f; // Base-game default: 1.0f

    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool handleWind = false;
    [Foldout(STRINGS.UNCHANGED_BY_ECC), ShowIf(nameof(handleWind))] public float windScalar = 1f;
}
