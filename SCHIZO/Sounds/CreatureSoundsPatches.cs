using HarmonyLib;
using UnityEngine;

namespace SCHIZO.Sounds;

[HarmonyPatch]
public static class CreatureSoundsPatches
{
    [HarmonyPatch(typeof(Pickupable), nameof(Pickupable.PlayPickupSound))]
    [HarmonyPostfix]
    public static void PlayCustomPickupSound(Pickupable __instance)
    {
        if (!CreatureSoundsHandler.TryGetCreatureSounds(__instance.GetTechType(), out CreatureSounds sounds)) return;
        if (!sounds.PickupSounds) return;

        sounds.PickupSounds.Play2D();
    }

    [HarmonyPatch(typeof(Pickupable), nameof(Pickupable.PlayDropSound))]
    [HarmonyPostfix]
    public static void PlayCustomDropSound(Pickupable __instance)
    {
        if (!CreatureSoundsHandler.TryGetCreatureSounds(__instance.GetTechType(), out CreatureSounds sounds)) return;
        if (!sounds.DropSounds) return;
        if (sounds.UnequipSounds) sounds.UnequipSounds.CancelAllDelayed();

        sounds.DropSounds.Play(__instance.GetComponent<FMOD_CustomEmitter>());
    }

    [HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.OnDraw))]
    [HarmonyPostfix]
    public static void PlayCustomDrawSound(PlayerTool __instance)
    {
        try
        {
            if (!__instance.pickupable || !CreatureSoundsHandler.TryGetCreatureSounds(__instance.pickupable.GetTechType(), out CreatureSounds sounds)) return;
            if (!sounds.EquipSounds) return;
            if (sounds.PickupSounds && Time.time < sounds.PickupSounds.LastPlay + 0.5f) return;

            sounds.EquipSounds.Play2D();
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
            if (!__instance.pickupable || !CreatureSoundsHandler.TryGetCreatureSounds(__instance.pickupable.GetTechType(), out CreatureSounds sounds)) return;
            if (!sounds.UnequipSounds) return;
            if (sounds.DropSounds && Time.time < sounds.DropSounds.LastPlay + 0.5f) return;
            if (sounds.EatSounds && Time.time < sounds.EatSounds.LastPlay + 0.5f) return;
            if (sounds.CraftSounds && Time.time < sounds.CraftSounds.LastPlay + 0.5f) return;

            sounds.UnequipSounds.Play2D(0.15f);
        }
        catch
        {
            // ignore
        }
    }

    [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Unlock))]
    [HarmonyPostfix]
    public static void PlayCustomScanSound(PDAScanner.EntryData entryData, bool unlockBlueprint, bool unlockEncyclopedia, bool verbose)
    {
        if (!CreatureSoundsHandler.TryGetCreatureSounds(entryData.key, out CreatureSounds sounds)) return;
        if (!verbose) return; // prevents scan sounds playing on loading screen
        if (!sounds.ScanSounds) return;

        sounds.ScanSounds.Play2D();
    }

    [HarmonyPatch(typeof(CraftData), nameof(CraftData.GetUseEatSound))]
    [HarmonyPostfix]
    public static void PlayCustomEatSound(TechType techType)
    {
        if (!CreatureSoundsHandler.TryGetCreatureSounds(techType, out CreatureSounds sounds)) return;
        if (!sounds.EatSounds) return;
        if (Time.time < sounds.EatSounds.LastPlay + 0.1f) return;
        if (sounds.UnequipSounds) sounds.UnequipSounds.CancelAllDelayed();

        sounds.EatSounds.Play2D();
    }

    [HarmonyPatch(typeof(Crafter), nameof(Crafter.OnCraftingBegin))]
    [HarmonyPostfix]
    public static void PlayCustomCookSound(TechType techType)
    {
        if (!CreatureSoundsHandler.TryGetCreatureSounds(techType, out CreatureSounds sounds)) return;
        if (!sounds.CraftSounds) return;
        if (sounds.UnequipSounds) sounds.UnequipSounds.CancelAllDelayed();

        sounds.CraftSounds.Play2D();
    }

    [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.NotifyAllAttachedDamageReceivers))]
    [HarmonyPostfix]
    public static void PlayCustomHurtSound(LiveMixin __instance, DamageInfo inDamage)
    {
        if (inDamage.damage == 0) return;
        Pickupable pickupable = __instance.GetComponent<Pickupable>();
        if (!pickupable || !CreatureSoundsHandler.TryGetCreatureSounds(pickupable.GetTechType(), out CreatureSounds sounds)) return;
        if (!sounds.HurtSounds) return;

        sounds.HurtSounds.Play(__instance.GetComponent<FMOD_CustomEmitter>());
    }
}
