using JetBrains.Annotations;
using TriInspector;
using UnityEditor;

namespace SCHIZO.Sounds.Collections
{
    partial class SoundCollection
    {
#if UNITY_EDITOR
        [Button("Create Instance"), UsedImplicitly]
        private void CreateInstance()
        {
            SoundCollectionInstance instance = CreateInstance<SoundCollectionInstance>();
            instance.collection = this;
            instance.bus = BusPaths.PDAVoice;
            instance.OnValidate();

            AssetDatabase.AddObjectToAsset(instance, this);
            Selection.activeObject = instance;

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
