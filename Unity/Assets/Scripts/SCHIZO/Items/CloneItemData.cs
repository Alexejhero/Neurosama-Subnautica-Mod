using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Items
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Clone Item Data")]
    // ReSharper disable once PartialTypeWithSinglePart
    public sealed partial class CloneItemData : ItemData
    {
        [BoxGroup("Common Properties"), ReadOnly, ValidateInput(nameof(loader_Validate))]
        public ItemLoader loader;

        [BoxGroup("Subnautica Data"), Label("Clone Target")]
        public TechType_All cloneTargetSN;

        [BoxGroup("Below Zero Data"), Label("Clone Target")]
        public TechType_All cloneTargetBZ;

#if !UNITY
        public TechType CloneTarget => (TechType) Helpers.RetargetHelpers.Pick(cloneTargetSN, cloneTargetBZ);
#endif

#if UNITY_EDITOR
        private void AssignItemLoader(ItemLoader itemLoader)
        {
            if (loader)
            {
                UnityEditor.AssetDatabase.RemoveObjectFromAsset(loader);
            }

            loader = itemLoader;

            if (loader)
            {
                loader.itemData = this;
                UnityEditor.AssetDatabase.AddObjectToAsset(loader, this);
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("Remove Loader")]
        private void DestroyLoader() => AssignItemLoader(null);
#endif

        #region NaughtyAttributes stuff

        private bool loader_Validate(ItemLoader val) => !autoRegister || val;

        #endregion
    }
}
