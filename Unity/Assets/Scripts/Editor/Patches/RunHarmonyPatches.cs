using HarmonyLib;
using UnityEditor;

public static class RunHarmonyPatches
{
    [InitializeOnLoadMethod]
    private static void Patch()
    {
        new Harmony("SCHIZO").PatchAll();
    }
}
