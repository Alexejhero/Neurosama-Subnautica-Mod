using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SCHIZO.Attributes;
using SCHIZO.Creatures.Components;
using SCHIZO.Creatures.Ermfish;
using SCHIZO.Events.Ermcon;

namespace SCHIZO.Patches;
internal static class DisableMoreComponentsInWaterPark
{
    [SuppressMessage("Style", "IDE0305:Simplify collection initialization",
        Justification = "no spans")]
    [InitializeMod]
    public static void AddComponentsToDisable()
    {
        Type[] arr = WaterParkCreature.behavioursToDisableInside.Concat([
            typeof(CarryCreature),
            typeof(ErmStack),
            typeof(ErmconAttendee),
            typeof(ErmconPanelist)
        ]).ToArray();
#if BELOWZERO
        WaterParkCreature.behavioursToDisableInside = arr;
#else
        // readonly smile
        HarmonyLib.AccessTools.Field(typeof(WaterParkCreature), nameof(WaterParkCreature.behavioursToDisableInside))
            .SetValue(null, arr);
#endif
    }
}
