using NaughtyAttributes;
using SCHIZO.Utilities;
using UnityEngine;

public class TrailManager : MonoBehaviour
{
    [Required] public Transform rootTransform;

    public Transform rootSegment;
    public Transform[] trails;
    public AnimationCurve pitchMultiplier;
    public AnimationCurve rollMultiplier;
    public AnimationCurve yawMultiplier;
    public float segmentSnapSpeed;
    public float maxSegmentOffset;
    public bool allowDisableOnScreen;

    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public BehaviourLOD levelOfDetail;
}
