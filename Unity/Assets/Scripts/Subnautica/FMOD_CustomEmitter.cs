using UnityEngine;

public class FMOD_CustomEmitter : MonoBehaviour
{
    public FMODAsset asset;
    public bool playOnAwake;
    public bool followParent = true;
    public bool restartOnPlay;
    [HideInInspector] public bool debug;
}
