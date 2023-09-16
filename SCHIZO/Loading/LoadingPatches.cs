using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SCHIZO.Creatures.Ermfish;
using SCHIZO.DataStructures;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SCHIZO.Loading;

[HarmonyPatch]
public static class LoadingPatches
{
    private record struct ArtWithCredit(Sprite art, string credit);

    private static readonly SavedRandomList<ArtWithCredit> _backgrounds = new("LoadingBackgrounds")
    {
        [1] = new ArtWithCredit(AssetLoader.GetUnitySprite("loading-bg-1.jpg"), "Art by P3R"),
        [2] = new ArtWithCredit(AssetLoader.GetUnitySprite("loading-bg-2.png"), "Art by yamplum"),
        [3] = new ArtWithCredit(AssetLoader.GetUnitySprite("loading-bg-3.png"), "Art by paccha (edit by MyBraza)"),
        [4] = new ArtWithCredit(AssetLoader.GetUnitySprite("loading-bg-4.png"), "Art by sugarph"),
        [5] = new ArtWithCredit(AssetLoader.GetUnitySprite("loading-bg-5.png"), "Art by paccha (edit by MyBraza)"),
        [6] = new ArtWithCredit(AssetLoader.GetUnitySprite("loading-bg-6.png"), "Art by paccha"),
        [7] = new ArtWithCredit(AssetLoader.GetUnitySprite("loading-bg-7.jpg"), "Art by P3R"),
        [8] = new ArtWithCredit(AssetLoader.GetUnitySprite("loading-bg-8.png"), "Art by Troobs"),
        [9] = new ArtWithCredit(AssetLoader.GetUnitySprite("loading-bg-9.png"), "Art by Troobs"),
        [10] = new ArtWithCredit(AssetLoader.GetUnitySprite("loading-bg-10.jpg"), "Art by 2Pfrog"),
    };

    private static TextMeshProUGUI _buildWatermark;
    private static bool _playedErmSound;

    [HarmonyPatch(typeof(uGUI_BuildWatermark), nameof(uGUI_BuildWatermark.UpdateText))]
    [HarmonyPrefix]
    public static bool ReplaceBuildWatermarkText(uGUI_BuildWatermark __instance) => false;

    [HarmonyPatch(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.Awake))]
    [HarmonyPostfix]
    public static void ChangeLoading(uGUI_SceneLoading __instance)
    {
        __instance.GetComponentInChildren<uGUI_Logo>().texture = AssetLoader.GetTexture("loading.png");

        ArtWithCredit art = _backgrounds.GetRandom();
        __instance.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = art.art;
        if (!_buildWatermark)
        {
            GameObject guiRoot = __instance.transform.root.gameObject;
            _buildWatermark = guiRoot.GetComponentInChildren<uGUI_BuildWatermark>()
                .GetComponent<TextMeshProUGUI>();
        }
        _buildWatermark.SetText(art.credit);
        _playedErmSound = false;
    }

    [HarmonyPatch(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.OnPreLayout))]
    [HarmonyPostfix]
    public static void HideBuildNumberInMenu(uGUI_SceneLoading __instance)
    {
        _buildWatermark.alpha = __instance.isLoading ? 0.7f : 0;
    }

    [HarmonyPatch(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.SetProgress))]
    [HarmonyPostfix]
    public static void PlayErmDuringLoadingScreen(uGUI_SceneLoading __instance, float value)
    {
        if (_playedErmSound) return;
        if (value > 0.5f)
        {
            _playedErmSound = true;
            ErmfishLoader.Sounds.ScanSounds.Play2D();
        }
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
