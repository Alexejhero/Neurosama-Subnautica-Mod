using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace SCHIZO.Jukebox;

[HarmonyPatch]
public static class JukeboxTrackUnlockPatches
{
    internal static readonly Dictionary<string, Sprite> CustomUnlockSprites = [];

    [HarmonyPatch(typeof(uGUI_PopupNotification), nameof(uGUI_PopupNotification.OnUnlockTrack))]
    [HarmonyPrefix]
    public static bool CustomSpritePatch(uGUI_PopupNotification __instance, string label)
    {
        if (!CustomUnlockSprites.TryGetValue(label, out Sprite sprite)) return true;

        uGUI_PopupNotification.Entry entry = new()
        {
            duration = __instance.defaultDuration,
            skin = PopupNotificationSkin.Unlock,
            title = Language.main.Get("JukeboxTrackUnlocked"),
            text = label,
            sprite = sprite,
            sound = __instance.soundJukeboxTrackUnlock
        };
        __instance.Enqueue(entry);

        return false;
    }
}
