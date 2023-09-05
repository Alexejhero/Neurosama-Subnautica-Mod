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
    private static readonly SavedRandomList<Sprite> _backgrounds = new("LoadingBackgrounds")
    {
        { 1, AssetLoader.GetUnitySprite("loading-bg-1.jpg") },
        { 2, AssetLoader.GetUnitySprite("loading-bg-2.png") },
        { 3, AssetLoader.GetUnitySprite("loading-bg-3.png") },
        { 4, AssetLoader.GetUnitySprite("loading-bg-4.png") },
        { 5, AssetLoader.GetUnitySprite("loading-bg-5.png") },
        { 6, AssetLoader.GetUnitySprite("loading-bg-6.png") },
    };

    [HarmonyPatch(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.Awake))]
    [HarmonyPostfix]
    public static void ChangeLoading(uGUI_SceneLoading __instance)
    {
        __instance.GetComponentInChildren<uGUI_Logo>().texture = AssetLoader.GetTexture("loading.png");
        __instance.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = _backgrounds.GetRandom();
    }

    [HarmonyPatch(typeof(SavingIndicator), nameof(SavingIndicator.OnEnable))]
    [HarmonyPrefix]
    public static void ChangeSaving(SavingIndicator __instance)
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
