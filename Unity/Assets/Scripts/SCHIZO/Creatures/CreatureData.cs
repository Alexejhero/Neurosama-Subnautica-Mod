using JetBrains.Annotations;
using NaughtyAttributes;
using SCHIZO.Enums.Subnautica;
using SCHIZO.Items;
using UnityEngine;

namespace SCHIZO.Creatures
{
    [CreateAssetMenu(menuName = "SCHIZO/Creatures/Creature Data")]
    public class CreatureData : ItemData
    {
        public bool isPickupable = false;

        [SerializeField, HideInInspector, UsedImplicitly] private ScriptableObject _liveMixin;

        // [BoxGroup("Creature Data")] public CreatureSoundData sounds; TODO
        [BoxGroup("Creature Data"), ValidateInput(nameof(Validate_behaviourType)), SerializeField] private BehaviourType_SN behaviourType;
        [BoxGroup("Creature Data")] public bool acidImmune = true;
        [BoxGroup("Creature Data")] public float bioReactorCharge = 0;

#if !UNITY
        public BehaviourType BehaviourType => (BehaviourType) behaviourType;
#endif

#if UNITY_EDITOR
        [ContextMenu("Create LiveMixinData")]
        private void CreateLiveMixinData()
        {
            // ReSharper disable once Unity.PreferGenericMethodOverload
            _liveMixin = CreateInstance("LiveMixinData");
            UnityEditor.AssetDatabase.AddObjectToAsset(_liveMixin, this);
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("Create LiveMixinData", true)]
        public bool CreateLiveMixinData_Validate() => !_liveMixin;

        [ContextMenu("Destroy LiveMixinData")]
        private void DestroyLiveMixinData()
        {
            UnityEditor.AssetDatabase.RemoveObjectFromAsset(_liveMixin);
            _liveMixin = null;
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("Destroy LiveMixinData", true)]
        public bool DestroyLiveMixinData_Validate() => _liveMixin;
#endif

        #region NaughyAttributes stuff

        protected override bool ShowPickupableProps() => isPickupable;

        private bool Validate_behaviourType(BehaviourType_SN val) => val != BehaviourType_SN.Unknown;

        #endregion
    }
}
