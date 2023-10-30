using UnityEngine;

namespace SCHIZO.Items.Data
{
    partial class CloneItemData
    {
#if UNITY_EDITOR
        private void AssignItemLoader(CloneItemLoader cloneItemLoader)
        {
            if (loader)
            {
                UnityEditor.AssetDatabase.RemoveObjectFromAsset(loader);
            }

            loader = cloneItemLoader;

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
