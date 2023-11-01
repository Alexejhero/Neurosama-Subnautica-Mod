using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using SCHIZO.Utilities;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    public partial class _CaveCrawler : _Creature
    {
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float animationMaxSpeed = 1;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float animationMaxTilt = 10;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float dampTime = 0.5f;
        [BoxGroup("Cave Crawler"), ExposedType("FMODAsset")] public ScriptableObject jumpSound;
        [BoxGroup("Cave Crawler"), ExposedType("FMOD_CustomLoopingEmitter")] public _FMOD_CustomEmitter walkingSound;
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public Rigidbody rb;
        [BoxGroup("Cave Crawler"), Required] public Collider aliveCollider;
        [BoxGroup("Cave Crawler"), Required] public Collider deadCollider;
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required, ExposedType("OnSurfaceTracker")] public MonoBehaviour onSurfaceTracker;
    }
}
