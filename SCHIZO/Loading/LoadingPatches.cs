using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SCHIZO.API.Unity.Loading;
using SCHIZO.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SCHIZO.Loading;

[HarmonyPatch]
public static class LoadingPatches
{
    private static TextMeshProUGUI _buildWatermark;
    private static string _artCredit;

    [HarmonyPatch(typeof(uGUI_BuildWatermark), nameof(uGUI_BuildWatermark.UpdateText))]
    [HarmonyPrefix]
    public static bool ReplaceBuildWatermarkText(uGUI_BuildWatermark __instance) => false;

    [HarmonyPatch(typeof(uGUI_BuildWatermark), nameof(uGUI_BuildWatermark.Start))]
    [HarmonyPostfix]
    public static void HookBuildWatermark(uGUI_BuildWatermark __instance)
    {
        _buildWatermark = __instance.text;
        _buildWatermark.SetText(_artCredit);
    }

    [HarmonyPatch(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.Awake))]
    [HarmonyPostfix]
    public static void ChangeLoading(uGUI_SceneLoading __instance)
    {
        ReplaceLogo(__instance.gameObject);

        LoadingBackground art = BackgroundLoader.LoadingBackgrounds.GetRandom();
        __instance.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = art.art;

        _artCredit = art.credit;
        if (_buildWatermark) _buildWatermark.SetText(_artCredit);
        ErmSoundDuringLoadingScreen._playedErmSound = false;
    }

#if BELOWZERO
    [HarmonyPatch(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.OnUpdate))]
#else
    [HarmonyPatch(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.OnPreLayout))]
#endif
    [HarmonyPostfix]
    public static void HideBuildNumberInMenu(uGUI_SceneLoading __instance)
    {
        if (_buildWatermark) _buildWatermark.alpha = __instance.isLoading ? 0.7f : 0;
    }

    [HarmonyPatch(typeof(SavingIndicator), nameof(SavingIndicator.OnEnable))]
    [HarmonyPrefix]
    public static void ChangeSaving(SavingIndicator __instance)
    {
        ReplaceLogo(__instance.gameObject);
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

    private static void ReplaceLogo(GameObject gui)
    {
        // temp until we have the BZ replacement
        uGUI_Logo logo = gui.GetComponentInChildren<uGUI_Logo>();
        if (logo) logo.texture = ResourceManager.LoadAsset<Texture2D>("loading icon");
    }
}
