using UnityEngine;

public class OnSurfaceTracker : MonoBehaviour
{
    [Range(0.0f, 180f)] public float maxSurfaceAngle = 60;
    public string ignoreTag;
    public bool trackSurfaceType;
}
