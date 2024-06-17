using JetBrains.Annotations;
using SCHIZO.Items.Data.Crafting;
using UnityEditor;
using UnityEngine;

namespace SCHIZO.Items.Data
{
    partial class ItemData
    {
        public Recipe RecipeSN => recipeSN;
        public Recipe RecipeBZ => recipeBZ;
        [SerializeField, HideInInspector, UsedImplicitly] private ScriptableObject _liveMixin;

#if UNITY_EDITOR

        [ContextMenu("Create LiveMixinData")]
        private void CreateLiveMixinData()
        {
            // ReSharper disable once Unity.PreferGenericMethodOverload
            _liveMixin = CreateInstance("LiveMixinData");
            AssetDatabase.AddObjectToAsset(_liveMixin, this);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            ProjectWindowUtil.ShowCreatedAsset(_liveMixin);
        }

        [ContextMenu("Create LiveMixinData", true)]
        public bool CreateLiveMixinData_Validate() => !_liveMixin;

        [ContextMenu("Destroy LiveMixinData")]
        private void DestroyLiveMixinData()
        {
            AssetDatabase.RemoveObjectFromAsset(_liveMixin);
            _liveMixin = null;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        [ContextMenu("Destroy LiveMixinData", true)]
        public bool DestroyLiveMixinData_Validate() => _liveMixin;
#endif
    }
}
