using SCHIZO.Attributes;
using SCHIZO.TriInspector;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    [DeclareFoldoutGroup(MODEL_PLUG_GROUP, Title = "Model Plug")]
    [DeclareFoldoutGroup(PLAYER_TOOL_GROUP + "/ik", Title = "IK")]
    [DeclareFoldoutGroup(PLAYER_TOOL_GROUP + "/anims", Title = "Main Animations")]
    [DeclareFoldoutGroup(PLAYER_TOOL_GROUP + "/bash", Title = "Bashing")]
    [DeclareFoldoutGroup(PLAYER_TOOL_GROUP + "/firstuse", Title = "First Use")]
    [DeclareFoldoutGroup(PLAYER_TOOL_GROUP + "/reload", Title = "Reloading")]
    [DeclareUnexploredGroup(PLAYER_TOOL_GROUP)]
    partial class _PlayerTool : _ModelPlug
    {
        protected const string PLAYER_TOOL_GROUP = "Player Tool";

        [GroupNext(PLAYER_TOOL_GROUP)]
        public Collider mainCollider;
        [ExposedType("Pickupable")] public MonoBehaviour pickupable;
        [ListDrawerSettings] public Renderer[] renderers;

        [GroupNext(PLAYER_TOOL_GROUP + "/ik")]
        public bool ikAimLeftArm;
        public bool ikAimRightArm;
        public Transform leftHandIKTarget;
        public Transform rightHandIKTarget;
        [Range(0.0f, 90f)] public float ikAimLookDownAngleLimit = 90;
        public bool useLeftAimTargetOnPlayer;

        [GroupNext(PLAYER_TOOL_GROUP + "/anims")]
        public bool hasAnimations = true;
        public float drawTime = 0.5f;
        public _FMODAsset drawSound;
        public _FMODAsset drawSoundUnderwater;
        public float holsterTime = 0.35f;
        public _FMODAsset holsterSoundAboveWater;
        public _FMODAsset holsterSoundUnderwater;
        public float dropTime = 1f;

        [GroupNext(PLAYER_TOOL_GROUP + "/bash")]
        public float bashTime = 0.7f;
        public float bleederDamage = 3;
        public float spikeyTrapDamage = 1;
        public _FMODAsset bashAnimationSound;
        public bool hasBashAnimation;
        public _FMODAsset hitBleederSound;

        [GroupNext(PLAYER_TOOL_GROUP + "/firstuse")]
        public bool hasFirstUseAnimation;
        public _FMOD_CustomEmitter firstUseSound;

        [GroupNext(PLAYER_TOOL_GROUP + "/reload")]
        public ReloadMode reloadMode = ReloadMode.Direct;
        public _FMODAsset reloadSound;

        [UnexploredGroupNext(PLAYER_TOOL_GROUP)]
        public Socket socket = Socket.RightHand;
        public bool waitForAnimDrawEvent;

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
