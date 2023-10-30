using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    partial class _FMOD_CustomEmitter : MonoBehaviour
    {
        [Foldout("Meaningless"), ExposedType("FMODAsset")] public ScriptableObject asset;
        [Foldout("Meaningless")] public bool playOnAwake;
        [Foldout("Meaningless")] public bool followParent = true;
        [Foldout("Meaningless")] public bool restartOnPlay;
        [Foldout("Meaningless")] public bool debug;
    }
}
