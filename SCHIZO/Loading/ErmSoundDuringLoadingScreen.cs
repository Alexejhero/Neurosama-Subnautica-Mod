using HarmonyLib;
using SCHIZO.API.Sounds;

namespace SCHIZO.Loading;

[HarmonyPatch]
public static class ErmSoundDuringLoadingScreen
{
    public static bool _playedErmSound;

    [HarmonyPatch(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.SetProgress))]
    [HarmonyPostfix]
    public static void PlayErmDuringLoadingScreen(uGUI_SceneLoading __instance, float value)
    {
        if (_playedErmSound) return;
        if (value > 0.5f)
        {
            _playedErmSound = true;
            CreatureSoundsHandler.GetCreatureSounds(ModItems.Ermfish).ScanSounds.Play2D();
        }
    }
}
