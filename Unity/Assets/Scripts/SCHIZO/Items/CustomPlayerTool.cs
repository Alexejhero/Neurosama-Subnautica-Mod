using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Unity.Items
{
    public partial class CustomPlayerTool
#if UNITY
    : MonoBehaviour
#endif
    {
        #region Unity compat (PlayerTool fields)
#if UNITY
        [Foldout("Player Tool")]
        public ReloadMode reloadMode = ReloadMode.Direct;
        [Foldout("Player Tool")]
        public Collider mainCollider;

        [ValidateInput(nameof(Validate_firstUseSound), "This must be an FMOD_CustomEmitter")]
        [Foldout("Player Tool")]
        public MonoBehaviour firstUseSound;
        [Foldout("Player Tool")]
        public float bleederDamage = 3f;
        [Foldout("Player Tool")]
        public float spikeyTrapDamage = 1f;
        [Foldout("Player Tool")]
        public Socket socket;
        [Foldout("Player Tool")]
        public bool ikAimRightArm;
        [Foldout("Player Tool")]
        public bool ikAimLeftArm;
        [Foldout("Player Tool")]
        public Transform rightHandIKTarget;
        [Foldout("Player Tool")]
        public bool useLeftAimTargetOnPlayer;
        [Foldout("Player Tool")]
        public Transform leftHandIKTarget;
        [Range(0f, 90f), Foldout("Player Tool"), Tooltip("BZ only")]
        public float ikAimLookDownAngleLimit = 90f;
        [Foldout("Player Tool")]
        public bool hasAnimations = true;
        [Foldout("Player Tool")]
        public float drawTime = 0.5f;
        [Foldout("Player Tool")]
        public float holsterTime = 0.35f;
        [Foldout("Player Tool")]
        public float dropTime = 1f;
        [Foldout("Player Tool")]
        public float bashTime = 0.7f;
        [Foldout("Player Tool")]
        [ValidateInput(nameof(Validate_pickupable), "This must be a Pickupable")]
        public MonoBehaviour pickupable;
        [Foldout("Player Tool")]
        public bool hasFirstUseAnimation;
        [Foldout("Player Tool")]
        public bool hasBashAnimation;
        [Foldout("Player Tool"), SerializeField]
        private bool waitForAnimDrawEvent;
        [Foldout("Player Tool")]
        public Renderer[] renderers;

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

        private bool Validate_firstUseSound() => !firstUseSound || firstUseSound.GetType().Name == "FMOD_CustomEmitter";
        private bool Validate_pickupable() => pickupable && pickupable.GetType().Name == "Pickupable";
        private bool ValidateRequiredFMODAsset(ScriptableObject obj) => obj && obj.GetType().Name == "FMODAsset";
        private bool ValidateFMODAsset(ScriptableObject obj) => !obj || obj.GetType().Name == "FMODAsset";
#endif
        #endregion Unity compat (PlayerTool fields)

        [Foldout("Custom Player Tool"), SerializeField]
        protected bool hasPrimaryUse;
        [Foldout("Custom Player Tool")]
        [ShowIf(nameof(hasPrimaryUse)), SerializeField]
        protected string primaryUseText;
        [Foldout("Custom Player Tool"), SerializeField]
        protected bool hasSecondaryUse;
        [Foldout("Custom Player Tool")]
        [ShowIf(nameof(hasSecondaryUse)), SerializeField]
        protected string secondaryUseText;
        [Foldout("Custom Player Tool"), SerializeField]
        protected bool hasAltUse;
        [Foldout("Custom Player Tool")]
        [ShowIf(nameof(hasAltUse)), SerializeField]
        protected string altUseText;
        [Foldout("Custom Player Tool")]
        [Label("(SN) Inherit Animations From"), SerializeField]
        private TechType_All inheritAnimationsFromSN;
        [Foldout("Custom Player Tool")]
        [Label("(BZ) Inherit Animations From"), SerializeField]
        private TechType_All inheritAnimationsFrom2; // bro...
    }
}
