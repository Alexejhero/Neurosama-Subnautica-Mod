using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SCHIZO.Attributes;
using SCHIZO.Creatures.Components;
using SCHIZO.Events.Ermcon;

namespace SCHIZO.Patches;
internal static class DisableMoreComponentsInWaterPark
{
    [SuppressMessage("Style", "IDE0305:Simplify collection initialization",
        Justification = "no spans")]
    [InitializeMod]
    public static void AddComponentsToDisable()
    {
        // uncomment this if you really *really* care about 10ns
        //int len = WaterParkCreature.behavioursToDisableInside.Length;
        //Type[] arr = new Type[len + 3];
        //WaterParkCreature.behavioursToDisableInside.CopyTo(arr, 0);
        //arr[len] = typeof(CarryCreature);
        //arr[len + 1] = typeof(ErmconAttendee);
        //arr[len + 2] = typeof(ErmconPanelist);
        Type[] arr = WaterParkCreature.behavioursToDisableInside.Concat([
            typeof(CarryCreature),
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
