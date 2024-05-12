using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace SCHIZO.Tweaks.Doom;

[HarmonyPatch]
public static class KeyHijackPatches
{
    private static readonly HashSet<KeyCode> _functionKeys = [
        KeyCode.F1,
        KeyCode.F2,
        KeyCode.F3,
        KeyCode.F4,
        KeyCode.F5,
        KeyCode.F6,
        KeyCode.F7,
        KeyCode.F8,
        KeyCode.F9,
        KeyCode.F10,
        KeyCode.F11,
        KeyCode.F12,
    ];
    public static bool SuppressKeyDownOnFunctionKeys { get; set; }

    [HarmonyPatch(typeof(Input), nameof(Input.GetKeyDown), [typeof(KeyCode)])]
    [HarmonyPostfix]
    public static void SuppressFunctionKeyDown(ref bool __result, KeyCode key)
    {
        if (!SuppressKeyDownOnFunctionKeys) return;
        if (!_functionKeys.Contains(key)) return;

        __result = false;
    }
}
