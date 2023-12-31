﻿using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

[DeclareComponentReferencesGroup]
public class TrailManager : MonoBehaviour
{
    [ComponentReferencesGroup, Required] public BehaviourLOD levelOfDetail;

    [Required] public Transform rootTransform;
    public Transform rootSegment;
    public Transform[] trails;
    public AnimationCurve pitchMultiplier;
    public AnimationCurve rollMultiplier;
    public AnimationCurve yawMultiplier;
    public float segmentSnapSpeed;
    public float maxSegmentOffset;
    public bool allowDisableOnScreen;
}
