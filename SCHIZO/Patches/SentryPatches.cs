using HarmonyLib;
using UnityEngine;

namespace SCHIZO.Patches;

[HarmonyPatch]
// disable sentry b/c i'm personally done spamming their servers with errors that aren't actually coming from the game
// (and also telemetry yuck)
public static class SentryPatches
{
    [HarmonyPatch(typeof(SentrySdk), nameof(SentrySdk.Start))]
    [HarmonyPostfix]
    public static void CeaseAndDesist(SentrySdk __instance)
    {
        __instance.enabled = false;
    }

    [HarmonyPatch(typeof(SentrySdkManager), nameof(SentrySdkManager.Awake))]
    [HarmonyPostfix]
    public static void PleaseAndThankYou(SentrySdkManager __instance)
    {
        Object.Destroy(__instance);
    }
}
