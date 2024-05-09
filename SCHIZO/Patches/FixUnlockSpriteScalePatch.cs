using HarmonyLib;
using UnityEngine;

namespace SCHIZO.Patches;

[HarmonyPatch]
public static class FixUnlockSpriteScalePatch
{
    [HarmonyPatch(typeof(uGUI_PopupNotificationDefault), nameof(uGUI_PopupNotificationDefault.OnShow))]
    [HarmonyPostfix]
    public static void UpdateSpriteRect(uGUI_PopupNotificationDefault __instance)
    {
        // basegame sprites:            Center: (0.0, 0.0, 0.0), Extents: (1.3, 0.6, 0.1)
        // (example) truckersfm sprite: Center: (0.0, 0.0, 0.0), Extents: (2.6, 2.6, 0.1)

        float width = 256f * 1.3f / __instance.image.sprite.bounds.extents.x;
        float offsetOffset = (256 - width) / 2;

        __instance.image.rectTransform.offsetMin = new Vector2(2 + offsetOffset, __instance.image.rectTransform.offsetMin.y);
        __instance.image.rectTransform.offsetMax = new Vector2(258 - offsetOffset, __instance.image.rectTransform.offsetMax.y);
    }
}
