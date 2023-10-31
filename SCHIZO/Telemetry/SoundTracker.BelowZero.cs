using FMOD.Studio;
using HarmonyLib;
using UnityEngine;

namespace SCHIZO.Telemetry;

[HarmonyPatch]
public partial class SoundTracker
{
    private partial bool IsPlayingSounds()
    {
        return Sounds is { _current.host: SoundHost.Encyclopedia or SoundHost.Log or SoundHost.Realtime };
    }
    [HarmonyPatch(typeof(Subtitles), nameof(global::Subtitles.OnPlaySound))]
    [HarmonyPostfix]
    public static void OnPlaySound(int id, EventInstance eventInstance, string sound, uint length)
    {
        //LOGGER.LogWarning($"{id} {eventInstance} {sound} {length}\n{StackTraceUtility.ExtractStackTrace()}");
    }
}
