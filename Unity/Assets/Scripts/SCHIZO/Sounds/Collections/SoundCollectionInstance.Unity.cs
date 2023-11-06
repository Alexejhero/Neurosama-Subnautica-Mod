using JetBrains.Annotations;
using TriInspector;
using UnityEditor;

namespace SCHIZO.Sounds.Collections
{
    partial class SoundCollectionInstance
    {
        protected override bool ShowDefaultProps => false;

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (collection == null) return;
            string oldName = name;
            name = $"{collection.name} ({bus})";
            if (oldName != name) AssetDatabase.SaveAssets();
        }

        [Button("Delete Instance"), UsedImplicitly]
        private void Delete()
        {
            Selection.activeObject = collection;
            AssetDatabase.RemoveObjectFromAsset(this);
            DestroyImmediate(this, true);

            EditorUtility.SetDirty(collection);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
