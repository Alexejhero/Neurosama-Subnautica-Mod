using SCHIZO.TriInspector;
using TriInspector;
using UnityEngine;

public class TrailManager : TriMonoBehaviour
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
