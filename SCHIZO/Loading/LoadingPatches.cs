using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace SCHIZO.Loading;

[HarmonyPatch]
public static class LoadingPatches
{
    [HarmonyPatch(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.Awake))]
    [HarmonyPostfix]
    public static void ChangeLoadingImage(uGUI_SceneLoading __instance)
    {
        __instance.GetComponentInChildren<uGUI_Logo>().texture = AssetLoader.GetTexture("loading.png");
    }

    [HarmonyPatch(typeof(uGUI_Logo), nameof(uGUI_Logo.Update))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> FixFumoRotation(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldc_R4 && (float) instruction.operand == 180)
            {
                instruction.operand = (float) 360;
            }

            yield return instruction;
        }
    }
}
