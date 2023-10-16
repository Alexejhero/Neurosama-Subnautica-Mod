using JetBrains.Annotations;
using NaughtyAttributes;
using SCHIZO.Enums.Subnautica;
using SCHIZO.Items.Data;
using UnityEngine;

namespace SCHIZO.Creatures
{
    [CreateAssetMenu(menuName = "SCHIZO/Creatures/Creature Data")]
    public partial class CreatureData : ItemData
    {
        public bool isPickupable = false;

        [BoxGroup("Creature Data"), ValidateInput(nameof(Validate_behaviourType)), SerializeField, UsedImplicitly] private BehaviourType_SN behaviourType;
        [BoxGroup("Creature Data")] public bool acidImmune = true;
        [BoxGroup("Creature Data")] public float bioReactorCharge = 0;

        #region NaughyAttributes stuff

        protected override bool ShowPickupableProps() => isPickupable;

        private bool Validate_behaviourType(BehaviourType_SN val) => val != BehaviourType_SN.Unknown;

        #endregion
    }
}
