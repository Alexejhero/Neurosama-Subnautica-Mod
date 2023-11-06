using UnityEditor;

namespace Editor.Scripts.Menus
{
    public static class SaveAssetDatabase
    {
        [MenuItem("SCHIZO/Save All Assets")]
        public static void SaveAllAssets()
        {
            AssetDatabase.SaveAssets();
        }
    }
}
