using Immersion.Trackers;

namespace Immersion.Patches;

[HarmonyPatch]
public static class LowOxygenAlertPatches
{
    [HarmonyPatch(typeof(HintSwimToSurface), nameof(HintSwimToSurface.ShouldShowWarning))]
    [HarmonyPostfix]
    private static void DisableHint(ref bool __result)
    {
        __result &= !(OxygenAlerts.Instance && OxygenAlerts.Instance.enabled);
    }
}
