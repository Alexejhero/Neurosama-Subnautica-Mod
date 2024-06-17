using System;
using SCHIZO.Attributes;
using SCHIZO.Creatures.Components;
using SCHIZO.Creatures.Ermfish;
using SCHIZO.Events.Ermcon;

namespace SCHIZO.Patches;
internal static class DisableMoreComponentsInWaterPark
{
    [InitializeMod]
    public static void AddComponentsToDisable()
    {
        Type[] arr =
        [
            ..WaterParkCreature.behavioursToDisableInside,
            typeof(CarryCreature),
            typeof(ErmStack),
            typeof(ErmconAttendee),
            typeof(ErmconPanelist),
        ];
#if BELOWZERO
        WaterParkCreature.behavioursToDisableInside = arr;
#else
        // readonly smile
        HarmonyLib.AccessTools.Field(typeof(WaterParkCreature), nameof(WaterParkCreature.behavioursToDisableInside))
            .SetValue(null, arr);
#endif
    }
}
