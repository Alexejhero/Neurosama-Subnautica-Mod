using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    partial class _PlayerTool : _ModelPlug
    {
        [Required] public Collider mainCollider;
        [ExposedType("Pickupable")] public MonoBehaviour pickupable;
        [ReorderableList] public Renderer[] renderers;

        [Foldout("IK")] public bool ikAimLeftArm;
        [Foldout("IK")] public bool ikAimRightArm;
        [Foldout("IK")] public Transform leftHandIKTarget;
        [Foldout("IK")] public Transform rightHandIKTarget;
        [Foldout("IK"), Range(0.0f, 90f)] public float ikAimLookDownAngleLimit = 90;
        [Foldout("IK")] public bool useLeftAimTargetOnPlayer;

        [Foldout("Main Animations")] public bool hasAnimations = true;
        [Foldout("Main Animations")] public float drawTime = 0.5f;
        [Foldout("Main Animations"), ExposedType("FMODAsset")] public Object drawSound;
        [Foldout("Main Animations"), ExposedType("FMODAsset")] public Object drawSoundUnderwater;
        [Foldout("Main Animations")] public float holsterTime = 0.35f;
        [Foldout("Main Animations"), ExposedType("FMODAsset")] public Object holsterSoundAboveWater;
        [Foldout("Main Animations"), ExposedType("FMODAsset")] public Object holsterSoundUnderwater;
        [Foldout("Main Animations")] public float dropTime = 1f;

        [Foldout("Bashing")] public float bashTime = 0.7f;
        [Foldout("Bashing")] public float bleederDamage = 3;
        [Foldout("Bashing")] public float spikeyTrapDamage = 1;
        [Foldout("Bashing"), ExposedType("FMODAsset")] public Object bashAnimationSound;
        [Foldout("Bashing")] public bool hasBashAnimation;
        [Foldout("Bashing"), ExposedType("FMODAsset")] public Object hitBleederSound;

        [Foldout("First Use")] public bool hasFirstUseAnimation;
        [Foldout("First Use"), ExposedType("FMOD_CustomEmitter")] public MonoBehaviour firstUseSound;

        [Foldout("Reloading")] public ReloadMode reloadMode = ReloadMode.Direct;
        [Foldout("Reloading"), ExposedType("FMODAsset")] public Object reloadSound;

        [Foldout("\u00af\\_(ツ)_/\u00af")] public Socket socket = Socket.RightHand;
        [Foldout("\u00af\\_(ツ)_/\u00af")] public bool waitForAnimDrawEvent;

        public enum ReloadMode
        {
            None,
            Direct,
            Animation
        }

        public enum Socket
        {
            RightHand,
            Camera
        }
    }
}
