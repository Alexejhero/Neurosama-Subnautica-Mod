using SCHIZO.Items.Data.Crafting;

namespace SCHIZO.Items.Data
{
    partial class ItemData
    {
        public Recipe RecipeSN => recipeSN;
        public Recipe RecipeBZ => recipeBZ;

#if UNITY_EDITOR
        protected void AssignItemLoader(ItemLoader itemLoader)
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

        [UnityEngine.ContextMenu("Remove Loader")]
        private void DestroyLoader() => AssignItemLoader(null);
#endif
    }
}
