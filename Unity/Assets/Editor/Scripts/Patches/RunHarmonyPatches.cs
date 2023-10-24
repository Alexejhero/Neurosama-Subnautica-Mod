using HarmonyLib;
using UnityEditor;

namespace Patches
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
