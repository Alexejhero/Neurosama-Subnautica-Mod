using HarmonyLib;
using SCHIZO.Events.ErmCon;
using UnityEngine;

namespace SCHIZO.Events;

[HarmonyPatch]
public static class CustomEventsLoader
{
    private static CustomEventManager eventManager;
    private static void LoadEvents(GameObject host)
    {
        eventManager = host.AddComponent<CustomEventManager>();

        // TODO: Maybe add an attribute here :)
        eventManager.AddEvent<ErmConEvent>();
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    [HarmonyPostfix]
    public static void EventLoadHook(Player __instance)
    {
        LoadEvents(Player.mainObject);
    }
}
