using SCHIZO.Attributes;
using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    [DeclareFoldoutGroup(CREATURE_GROUP, Title = "Creature")]
    [DeclareUnexploredGroup(CAVE_CRAWLER_GROUP)]
    public partial class _CaveCrawler : _Creature
    {
        [ComponentReferencesGroup, Required] public Rigidbody rb;
        [ComponentReferencesGroup, Required, ExposedType("OnSurfaceTracker")] public MonoBehaviour onSurfaceTracker;

        [Group(CAVE_CRAWLER_GROUP), ExposedType("FMODAsset")] public ScriptableObject jumpSound;
        [Group(CAVE_CRAWLER_GROUP), ExposedType("FMOD_CustomLoopingEmitter")] public _FMOD_CustomEmitter walkingSound;
        [Group(CAVE_CRAWLER_GROUP), Required] public Collider aliveCollider;
        [Group(CAVE_CRAWLER_GROUP), Required] public Collider deadCollider;

        [UnexploredGroup(CAVE_CRAWLER_GROUP)] public float animationMaxSpeed = 1;
        [UnexploredGroup(CAVE_CRAWLER_GROUP)] public float animationMaxTilt = 10;
        [UnexploredGroup(CAVE_CRAWLER_GROUP)] public float dampTime = 0.5f;
    }
}
