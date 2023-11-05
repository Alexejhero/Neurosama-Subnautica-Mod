using SCHIZO.Attributes.Typing;
using SCHIZO.TriInspector;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    [DeclareFoldoutGroup(DROP_TOOL_GROUP, Title = "Drop Tool")]
    [DeclareFoldoutGroup(CREATURE_TOOL_GROUP + "/release", Title = "Release")]
    [DeclareUnexploredGroup(CREATURE_TOOL_GROUP)]
    partial class _CreatureTool : _DropTool
    {
        protected const string CREATURE_TOOL_GROUP = "creaturetool";

        [GroupNext(CREATURE_TOOL_GROUP + "/release")]
        public _FMODAsset releaseUnderwaterSound;
        public _FMODAsset releaseAboveWaterSound;

        [UnexploredGroupNext(CREATURE_TOOL_GROUP)]
        public bool useSpeedAnimationParam;
        public bool disableSoundsOnKill = true;

        [ComponentReferencesGroupNext]
        [Required] public _Creature creature;
        [Required] public Animator animator;
        [Required, ExposedType("Locomotion")] public MonoBehaviour locomotion;
        [Required, ExposedType("LiveMixin")] public MonoBehaviour liveMixin;
    }
}
