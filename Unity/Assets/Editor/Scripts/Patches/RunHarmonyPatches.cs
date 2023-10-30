using HarmonyLib;
using UnityEditor;

namespace Editor.Scripts.Patches
{
    public static class RunHarmonyPatches
    {
        [InitializeOnLoadMethod]
        private static void Patch()
        {
            new Harmony("SCHIZO").PatchAll();
        }
    }
}
