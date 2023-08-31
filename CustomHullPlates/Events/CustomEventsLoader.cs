using HarmonyLib;
using UnityEngine;

namespace SCHIZO.Events
{
    [HarmonyPatch]
    public static class CustomEventsLoader
    {
        public static CustomEventManager eventManager;
        public static void LoadEvents(GameObject host)
        {
            eventManager = host.AddComponent<CustomEventManager>();

            eventManager.AddEvent<ErmMoonEvent>();
        }
        
        [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
        [HarmonyPostfix]
        public static void EventLoadHook(Player __instance)
        {
            LoadEvents(Player.mainObject);
        }
    }
}
