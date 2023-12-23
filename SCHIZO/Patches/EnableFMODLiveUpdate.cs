using FMODUnity;
using HarmonyLib;

namespace SCHIZO.Patches;

[HarmonyPatch]
public static class EnableFMODLiveUpdate
{
#if DEBUG_FMOD
    [HarmonyPatch(typeof(Platform), nameof(Platform.IsLiveUpdateEnabled), MethodType.Getter)]
    [HarmonyPrefix]
#endif
    public static bool EnableLiveUpdate(RuntimeManager __instance, out bool __result)
    {
        __result = true;
        return false;
    }
    // mostly useless information like cpu/ram usage
    //[HarmonyPatch(typeof(Platform), nameof(Platform.IsOverlayEnabled), MethodType.Getter)]
    //[HarmonyPrefix]
    public static bool EnableOverlay(RuntimeManager __instance, out bool __result)
    {
        __result = true;
        return false;
    }
}
