using UnityEditor;
using UnityEngine;
using System.IO;
public class AddPrefabToTemplate
{
    private static string templateFolderPath = "Assets/Templates/";

    [MenuItem("Assets/Create Creature Prefab")]
    public static void CreateCreaturePrefab()
    {
        string selectionPath = AssetDatabase.GetAssetPath(Selection.activeGameObject);
        string outputFolderPath = Path.GetDirectoryName(selectionPath) + "/";

        GameObject selectedObject = PrefabUtility.LoadPrefabContents(selectionPath);
        GameObject template = (GameObject) PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(templateFolderPath + "Creature Prefab Template.prefab"));
        string newAssetPath = AssetDatabase.GenerateUniqueAssetPath(outputFolderPath + selectedObject.name + ".prefab");
        selectedObject.transform.SetParent(template.transform, false);
        PrefabUtility.SaveAsPrefabAssetAndConnect(template, newAssetPath, InteractionMode.UserAction);
        GameObject.DestroyImmediate(template);
        //PrefabUtility.UnloadPrefabContents(selectedObject); ???????
    }

    [MenuItem("Assets/Create Creature Prefab", true)]
    public static bool ValidateCreateCreaturePrefab()
    {
        if (Selection.activeObject != null && Selection.objects.Length == 1 && Selection.activeObject.GetType().ToString() == "UnityEngine.GameObject")
            return true;
        return false;
    }

    [MenuItem("Assets/Create Constructable Prefab")]
    public static void CreateConstructablePrefab()
    {
        string selectionPath = AssetDatabase.GetAssetPath(Selection.activeGameObject);
        string outputFolderPath = Path.GetDirectoryName(selectionPath) + "/";

        GameObject selectedObject = PrefabUtility.LoadPrefabContents(selectionPath);
        GameObject template = (GameObject) PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(templateFolderPath + "Constructable Prefab Template.prefab"));
        string newAssetPath = AssetDatabase.GenerateUniqueAssetPath(outputFolderPath + selectedObject.name + ".prefab");
        selectedObject.transform.SetParent(template.transform, false);
        PrefabUtility.SaveAsPrefabAssetAndConnect(template, newAssetPath, InteractionMode.UserAction);
        GameObject.DestroyImmediate(template);
        //PrefabUtility.UnloadPrefabContents(selectedObject); ?????
    }

    [MenuItem("Assets/Create Constructable Prefab", true)]
    public static bool ValidateCreateConstructablePrefab()
    {
        if (Selection.activeObject != null && Selection.objects.Length == 1 && Selection.activeObject.GetType().ToString() == "UnityEngine.GameObject")
            return true;
        return false;
    }
}
