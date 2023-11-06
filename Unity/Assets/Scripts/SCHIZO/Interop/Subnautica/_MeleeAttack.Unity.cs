using SCHIZO.Attributes;
using TriInspector;
using SCHIZO.TriInspector.Attributes;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    [DeclareComponentReferencesGroup]
    [DeclareUnexploredGroup]
    partial class _MeleeAttack : MonoBehaviour
    {
        [ComponentReferencesGroup, Required, ExposedType("LastTarget")] public MonoBehaviour lastTarget;
        [ComponentReferencesGroup, Required] public _Creature creature;
        [ComponentReferencesGroup, Required, ExposedType("LiveMixin")] public MonoBehaviour liveMixin;
        [ComponentReferencesGroup, Required] public Animator animator;

        [Required] public GameObject mouth;
        public float biteInterval = 1;
        public float biteDamage = 30;
        public bool canBitePlayer = true;
        public bool canBiteVehicle = false;
        public bool canBiteCyclops = false;
        public bool canBiteCreature = true;
        public bool ignoreSameKind = false;
        public bool canBeFed = true;

        [UnexploredGroup] public float biteAggressionThreshold = 0.3f;
        [UnexploredGroup] public float eatHungerDecrement = 0.5f;
        [UnexploredGroup] public float eatHappyIncrement = 0.5f;
        [UnexploredGroup] public float biteAggressionDecrement = 0.4f;
        [UnexploredGroup] public GameObject damageFX;

        // [UnexploredGroup, ReadOnly] public Object attackSound;

        public void OnTouch(Collider col) {}
    }
}
