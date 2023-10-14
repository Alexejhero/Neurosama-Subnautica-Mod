using NaughtyAttributes;
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
        [BoxGroup("Creature Data")] public bool acidImmune = true;
        [BoxGroup("Creature Data")] public float bioReactorCharge = 0;

        #region NaughyAttributes stuff

        protected override bool ShowPickupableProps() => isPickupable;

        #endregion
    }
}
