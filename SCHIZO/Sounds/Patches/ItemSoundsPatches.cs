using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SCHIZO.Helpers;
using SCHIZO.Sounds.Players;
using UnityEngine;

namespace SCHIZO.Sounds.Patches;

[HarmonyPatch]
public static class ItemSoundsPatches
{
    [HarmonyPatch(typeof(Pickupable), nameof(Pickupable.PlayPickupSound))]
    [HarmonyPostfix]
    public static void PlayCustomPickupSound(Pickupable __instance)
    {
        if (!ItemSounds.TryGet(__instance.GetTechType(), out ItemSounds itemSounds)) return;
        itemSounds.pickupSounds!?.Play2D();
    }

    [HarmonyPatch(typeof(Pickupable), nameof(Pickupable.PlayDropSound))]
    [HarmonyPostfix]
    public static void PlayCustomDropSound(Pickupable __instance)
    {
        if (!ItemSounds.TryGet(__instance.GetTechType(), out ItemSounds sounds)) return;
        if (sounds.dropSounds == null) return;

        sounds.holsterSounds!?.CancelAllDelayed();
        sounds.dropSounds.Play(__instance.GetComponent<FMOD_CustomEmitter>());

        __instance.GetComponentsInChildren<InventoryAmbientSoundPlayer>().ForEach(p => p.Stop());
    }

    [HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.OnDraw))]
    [HarmonyPostfix]
    public static void PlayCustomDrawSound(PlayerTool __instance)
    {
        try
        {
            if (!__instance.pickupable || !ItemSounds.TryGet(__instance.pickupable.GetTechType(), out ItemSounds sounds)) return;
            if (sounds.drawSounds == null) return;

            if (Time.time < sounds.pickupSounds!?.LastPlay + 0.5f) return;

            sounds.drawSounds.Play2D();
        }
        catch
        {
            // ignore
        }
    }

    [HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.OnHolster))]
    [HarmonyPostfix]
    public static void PlayCustomHolsterSound(PlayerTool __instance)
    {
        try
        {
            if (!__instance.pickupable || !ItemSounds.TryGet(__instance.pickupable.GetTechType(), out ItemSounds sounds)) return;
            if (sounds.holsterSounds == null) return;

            if (Time.time < sounds.dropSounds!?.LastPlay + 0.5f) return;
            if (Time.time < sounds.eatSounds!?.LastPlay + 0.5f) return;
            if (Time.time < sounds.cookSounds!?.LastPlay + 0.5f) return;

            sounds.holsterSounds.Play2D(0.15f);
        }
        catch
        {
            // ignore
        }
    }

    [HarmonyPatch(typeof(Survival), nameof(Survival.Eat))]
    public static class PlayCustomEatSound
    {
        private static readonly MethodInfo _target =
#if BELOWZERO
            AccessTools.Method(typeof(Utils), nameof(Utils.PlayFMODAsset), new[] { typeof(FMODAsset), typeof(Vector3), typeof(float)});
#else
            AccessTools.Method(typeof(FMODUWE), nameof(FMODUWE.PlayOneShot), new[] {typeof(string), typeof(Vector3), typeof(float)});
#endif

        [HarmonyTranspiler, UsedImplicitly]
        public static IEnumerable<CodeInstruction> Injector(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new(instructions);
            matcher.Start();
            matcher.SearchForward(instr => instr.Calls(_target));
            matcher.Advance(1);
            matcher.InsertAndAdvance
            (
                new CodeInstruction(OpCodes.Ldloc_S, IS_BELOWZERO ? 7 : 2),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayCustomEatSound), nameof(Patch)))
            );
            return matcher.InstructionEnumeration();
        }

        private static void Patch(TechType techType)
        {
            if (!ItemSounds.TryGet(techType, out ItemSounds sounds)) return;
            if (sounds.eatSounds == null) return;

            if (Time.time < sounds.eatSounds.LastPlay + 0.1f) return;

            sounds.holsterSounds!?.CancelAllDelayed();
            sounds.eatSounds.Play2D();
        }
    }

    [HarmonyPatch(typeof(Crafter), nameof(Crafter.OnCraftingBegin))]
    [HarmonyPostfix]
    public static void PlayCustomCookSound(TechType techType)
    {
        if (!ItemSounds.TryGet(techType, out ItemSounds sounds)) return;
        if (sounds.cookSounds == null) return;

        sounds.holsterSounds!?.CancelAllDelayed();
        sounds.cookSounds.Play2D();
    }

    [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.Kill))]
    [HarmonyPostfix]
    public static void PlayPlayerDeathSound(LiveMixin __instance)
    {
        if (Player.main.liveMixin != __instance) return;
        foreach (InventoryItem item in Inventory.main.container.GetAllItems())
        {
            if (!ItemSounds.TryGet(item.techType, out ItemSounds sounds)) continue;
            sounds.playerDeathSounds!?.Play2D(0.15f);
        }
    }
}
