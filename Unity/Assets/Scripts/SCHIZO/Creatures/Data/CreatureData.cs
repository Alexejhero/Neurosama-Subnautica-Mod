using SCHIZO.Attributes;
using SCHIZO.Interop.Subnautica.Enums.Subnautica;
using SCHIZO.Items.Data;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Creatures.Data
{
    [CreateAssetMenu(menuName = "SCHIZO/Creatures/Creature Data")]
    [DeclareBoxGroup("creaturedata", Title = "Creature Data")]
    public partial class CreatureData : ItemData
    {
        [PropertyOrder(1), Careful] public bool isPickupable;

        [Group("creaturedata"), ValidateInput(nameof(Validate_behaviourType)), SerializeField]
        private BehaviourType_SN behaviourType;

        [Group("creaturedata")]
        public bool acidImmune = true;

        [Group("creaturedata")]
        public float bioReactorCharge;

        [Group("creaturedata")]
        public ModdedWaterParkCreatureData waterParkData;

        #region TriInspector stuff

        protected override bool ShowPickupableProps() => isPickupable;

        private TriValidationResult Validate_behaviourType()
        {
            if (behaviourType == BehaviourType_SN.Unknown) return TriValidationResult.Error("Behaviour type cannot be Unknown");
            return TriValidationResult.Valid;
        }

        #endregion

        private CreatureData()
        {
            loader = new CreatureLoader();
        }
    }
}
