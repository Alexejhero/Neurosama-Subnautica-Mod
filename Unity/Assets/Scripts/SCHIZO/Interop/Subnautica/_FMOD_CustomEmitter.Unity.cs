using TriInspector;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    [DeclareFoldoutGroup("Meaningless")]
    partial class _FMOD_CustomEmitter : MonoBehaviour
    {
        [Group("Meaningless")] public _FMODAsset asset;
        [Group("Meaningless")] public bool playOnAwake;
        [Group("Meaningless")] public bool followParent = true;
        [Group("Meaningless")] public bool restartOnPlay;
        [Group("Meaningless")] public bool debug;
    }
}
