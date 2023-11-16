using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace SCHIZO.Creatures.Data
{
    partial class CreatureData
    {
        [SerializeField, HideInInspector, UsedImplicitly] private ScriptableObject _liveMixin;

#if UNITY_EDITOR

        [ContextMenu("Create LiveMixinData")]
        private void CreateLiveMixinData()
        {
            // ReSharper disable once Unity.PreferGenericMethodOverload
            _liveMixin = CreateInstance("LiveMixinData");
            AssetDatabase.AddObjectToAsset(_liveMixin, this);
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("Create LiveMixinData", true)]
        public bool CreateLiveMixinData_Validate() => !_liveMixin;

        [ContextMenu("Destroy LiveMixinData")]
        private void DestroyLiveMixinData()
        {
            AssetDatabase.RemoveObjectFromAsset(_liveMixin);
            _liveMixin = null;
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("Destroy LiveMixinData", true)]
        public bool DestroyLiveMixinData_Validate() => _liveMixin;
#endif
    }
}
