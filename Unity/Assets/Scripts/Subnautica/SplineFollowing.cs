﻿using NaughtyAttributes;
using UnityEngine;

public class SplineFollowing : MonoBehaviour
{
    [Required] public Locomotion locomotion;
    [Required] public Rigidbody useRigidbody;
    [Required] public BehaviourLOD levelOfDetail;

    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float targetRange = 1;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float lookAhead = 1;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool respectLOD;

    [HideInInspector] public float inertia = 1;
}
