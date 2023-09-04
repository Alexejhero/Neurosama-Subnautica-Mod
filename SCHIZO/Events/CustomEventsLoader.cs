using HarmonyLib;
using SCHIZO.Events.ErmCon;
using SCHIZO.Events.ErmMoon;
using SCHIZO.Events.ErmRapture;
using UnityEngine;

namespace SCHIZO.Events
{
    [HarmonyPatch]
    public static class CustomEventsLoader
    {
        private static CustomEventManager eventManager;
        private static void LoadEvents(GameObject host)
        {
            eventManager = host.AddComponent<CustomEventManager>();

            eventManager.AddEvent<ErmMoonEvent>();
            eventManager.AddEvent<ErmConEvent>();
            eventManager.AddEvent<ErmRaptureEvent>();
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
        [HarmonyPostfix]
        public static void EventLoadHook(Player __instance)
        {
            LoadEvents(Player.mainObject);
        }
    }
}
