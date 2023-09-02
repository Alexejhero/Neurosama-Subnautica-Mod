using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SCHIZO.DataStructures;
using UnityEngine;
using UnityEngine.UI;

namespace SCHIZO.Loading;

[HarmonyPatch]
public static class LoadingPatches
{
    private static readonly RandomList<Sprite> _backgrounds = new()
    {
        AssetLoader.GetUnitySprite("loading-bg-1.jpg"),
        AssetLoader.GetUnitySprite("loading-bg-2.png"),
        AssetLoader.GetUnitySprite("loading-bg-3.png"),
        AssetLoader.GetUnitySprite("loading-bg-4.png"),
        AssetLoader.GetUnitySprite("loading-bg-5.png"),
    };

    [HarmonyPatch(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.Awake))]
    [HarmonyPostfix]
    public static void ChangeLoadingImage(uGUI_SceneLoading __instance)
    {
        __instance.GetComponentInChildren<uGUI_Logo>().texture = AssetLoader.GetTexture("loading.png");
        __instance.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = _backgrounds.GetRandom();
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
