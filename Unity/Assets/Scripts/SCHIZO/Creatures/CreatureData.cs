using NaughtyAttributes;
using SCHIZO.Unity.Enums.Subnautica;
using SCHIZO.Unity.Items;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Creatures
{
    [CreateAssetMenu(menuName = "SCHIZO/Creatures/Creature Data")]
    public class CreatureData : ItemData
    {
        public bool isPickupable = false;

        // [BoxGroup("Creature Data")] public CreatureSoundData sounds; TODO
        [BoxGroup("Creature Data"), ValidateInput(nameof(Validate_behaviourType)), SerializeField] private BehaviourType_SN behaviourType;
        [BoxGroup("Creature Data")] public bool acidImmune = true;
        [BoxGroup("Creature Data")] public float bioReactorCharge = 0;

#if !UNITY
        public BehaviourType BehaviourType => (BehaviourType) behaviourType;
#endif

        #region NaughyAttributes stuff

        protected override bool ShowPickupableProps() => isPickupable;

        private bool Validate_behaviourType(BehaviourType_SN val) => val != BehaviourType_SN.Unknown;

        #endregion
    }
}
