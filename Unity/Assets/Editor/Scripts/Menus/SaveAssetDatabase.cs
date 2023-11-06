using UnityEditor;

namespace Editor.Scripts.Menus
{
    public static class SaveAssetDatabase
    {
        [MenuItem("Tools/SCHIZO/Save All Assets")]
        public static void SaveAllAssets()
        {
            AssetDatabase.SaveAssets();
        }
    }
}
