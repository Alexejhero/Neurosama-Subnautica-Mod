using UnityEngine;

namespace SCHIZO.Items.Data
{
    partial class CloneItemData
    {
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
    }
}
