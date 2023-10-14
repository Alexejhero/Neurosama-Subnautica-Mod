using UnityEngine;

public static class ECCLIBRARY
{
    public static AnimationCurve maxRangeMultiplierCurve = new AnimationCurve(new Keyframe[3]
    {
        new Keyframe(0.0f, 1f, 0.0f, 0.0f, 0.333f, 0.333f),
        new Keyframe(0.5f, 0.5f, 0.0f, 0.0f, 0.333f, 0.333f),
        new Keyframe(1f, 1f, 0.0f, 0.0f, 0.333f, 0.333f)
    });

    public static AnimationCurve distanceAggressionMultiplierCurve = new AnimationCurve(new Keyframe[2]
    {
        new Keyframe(0.0f, 1f, 0.0f, 0.0f, 0.333f, 0.333f),
        new Keyframe(1f, 0.0f, -3f, -3f, 0.333f, 0.333f)
    });
}
