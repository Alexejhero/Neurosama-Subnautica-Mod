using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using SCHIZO.Utilities;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    partial class _CreatureTool : _DropTool
    {
        [Foldout("Release"), ExposedType("FMODAsset")] public Object releaseUnderwaterSound;
        [Foldout("Release"), ExposedType("FMODAsset")] public Object releaseAboveWaterSound;

        [Foldout("\u00af\\_(ツ)_/\u00af")] public bool useSpeedAnimationParam;
        [Foldout("\u00af\\_(ツ)_/\u00af")] public bool disableSoundsOnKill = true;

        [Foldout(STRINGS.COMPONENT_REFERENCES), Required, ExposedType("Creature")] public MonoBehaviour creature;
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public Animator animator;
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required, ExposedType("Locomotion")] public MonoBehaviour locomotion;
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required, ExposedType("LiveMixin")] public MonoBehaviour liveMixin;
    }
}
