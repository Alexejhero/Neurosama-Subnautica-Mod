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
    private record struct ArtWithCredit(Sprite art, string credit);
    private static readonly SavedRandomList<ArtWithCredit> _backgrounds = new("LoadingBackgrounds")
    {
        [1] = new(AssetLoader.GetUnitySprite("loading-bg-1.jpg"), "Art by P3R"),
        [2] = new(AssetLoader.GetUnitySprite("loading-bg-2.png"), "Art by yamplum"),
        [3] = new(AssetLoader.GetUnitySprite("loading-bg-3.png"), "Art by paccha (edit by MyBraza)"),
        [4] = new(AssetLoader.GetUnitySprite("loading-bg-4.png"), "Art by sugarph"),
        [5] = new(AssetLoader.GetUnitySprite("loading-bg-5.png"), "Art by MyBraza"),
        [6] = new(AssetLoader.GetUnitySprite("loading-bg-6.png"), "Art by paccha"),
        [7] = new(AssetLoader.GetUnitySprite("loading-bg-7.jpg"), "Art by P3R"),
        [8] = new(AssetLoader.GetUnitySprite("loading-bg-8.png"), "Art by Troobs"),
    };

    private static uGUI_BuildWatermark build;
    private static string artCredit = "";

    [HarmonyPatch(typeof(uGUI_BuildWatermark), nameof(uGUI_BuildWatermark.UpdateText))]
    [HarmonyPrefix]
    public static bool ReplaceBuildWatermarkText(uGUI_BuildWatermark __instance)
    {
        build = __instance;
        __instance.text.text = artCredit;
        return false;
    }

    [HarmonyPatch(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.Awake))]
    [HarmonyPostfix]
    public static void ChangeLoading(uGUI_SceneLoading __instance)
    {
        __instance.GetComponentInChildren<uGUI_Logo>().texture = AssetLoader.GetTexture("loading.png");
        (Sprite bg, string credit) = _backgrounds.GetRandom();
        __instance.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = bg;
        artCredit = credit;
        if (!build) return;
        build.UpdateText();
    }

    [HarmonyPatch(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.OnPreLayout))]
    [HarmonyPostfix]
    public static void HideBuildNumberInMenu(uGUI_SceneLoading __instance)
    {
        build.gameObject.SetActive(__instance.isLoading);
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
