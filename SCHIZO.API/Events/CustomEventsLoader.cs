using HarmonyLib;
using SCHIZO.API.Attributes;
using UnityEngine;

namespace SCHIZO.API.Events;

[HarmonyPatch]
public static class CustomEventsLoader
{
    private static CustomEventManager eventManager;
    private static void LoadEvents(GameObject host)
    {
        eventManager = host.AddComponent<CustomEventManager>();

        LoadEventAttribute.AddAll(eventManager);
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    [HarmonyPostfix]
    public static void EventLoadHook(Player __instance)
    {
        LoadEvents(Player.mainObject);
    }
}
