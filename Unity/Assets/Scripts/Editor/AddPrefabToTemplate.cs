using UnityEditor;
using UnityEngine;
using System.IO;

public class AddPrefabToTemplate
{
    private static string templateFolderPath = "Assets/Templates/";
    private enum types
    {
        Creature,
        Constructable,
    }

    private static void createPrefab(types t, string localPath)
    {
        string outputFolderPath = Path.GetDirectoryName(localPath) + "/";
        GameObject selectedObject = PrefabUtility.LoadPrefabContents(localPath);
        GameObject template = (GameObject) PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(templateFolderPath + t.ToString() + " Prefab Template.prefab"));
        string resultPrefabNameAndPath = AssetDatabase.GenerateUniqueAssetPath(outputFolderPath + selectedObject.name + ".prefab");
        selectedObject.transform.SetParent(template.transform, false);
        PrefabUtility.SaveAsPrefabAssetAndConnect(template, resultPrefabNameAndPath, InteractionMode.AutomatedAction);
        GameObject.DestroyImmediate(template);
    }

    [MenuItem("Assets/Create Creature Prefab")]
    public static void CreateCreaturePrefab()
    {
        createPrefab(types.Creature, AssetDatabase.GetAssetPath(Selection.activeGameObject));
    }

    [MenuItem("Assets/Create Creature Prefab", true)]
    public static bool ValidateCreateCreaturePrefab()
    {
        if (Selection.activeObject != null && Selection.objects.Length == 1 && Selection.activeObject is GameObject)
            return true;
        return false;
    }

    [MenuItem("Assets/Create Constructable Prefab")]
    public static void CreateConstructablePrefab()
    {
        createPrefab(types.Constructable, AssetDatabase.GetAssetPath(Selection.activeGameObject));
    }

    [MenuItem("Assets/Create Constructable Prefab", true)]
    public static bool ValidateCreateConstructablePrefab()
    {
        if (Selection.activeObject != null && Selection.objects.Length == 1 && Selection.activeObject is GameObject)
            return true;
        return false;
    }
}
