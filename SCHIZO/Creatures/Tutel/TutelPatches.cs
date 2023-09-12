using System.Linq;
using HarmonyLib;
using Nautilus.Handlers;
using UnityEngine;

namespace SCHIZO.Creatures.Tutel;

[HarmonyPatch]
public static class TutelPatches
{
	[HarmonyPatch(typeof(Pickupable), nameof(Pickupable.PlayPickupSound))]
	[HarmonyPostfix]
	public static void PlayTutelPickupSound(Pickupable __instance)
	{
		if (!TutelLoader.TutelTechTypes.Contains(__instance.GetTechType())) return;
		TutelLoader.PickupSounds.Play2D();
	}

	[HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Unlock))]
	[HarmonyPostfix]
	public static void PlayTutelScanSound(PDAScanner.EntryData entryData)
	{
		if (!TutelLoader.TutelTechTypes.Contains(entryData.key)) return;
		TutelLoader.ScanSounds.Play2D();
	}

	[HarmonyPatch(typeof(CraftData), nameof(CraftData.GetUseEatSound))]
	[HarmonyPostfix]
	public static void PlayTutelEatSound(TechType techType)
	{
		if (!TutelLoader.TutelTechTypes.Contains(techType)) return;
		if (Time.time < TutelLoader.EatSounds.LastPlay + 0.1f) return;
		TutelLoader.EatSounds.Play2D();
	}

	[HarmonyPatch(typeof(Crafter), nameof(Crafter.OnCraftingBegin))]
	[HarmonyPostfix]
	public static void PlayTutelCookSound(TechType techType)
	{
		if (!TutelLoader.TutelTechTypes.Contains(techType)) return;
		TutelLoader.CraftSounds.Play2D();
	}

	[HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.NotifyAllAttachedDamageReceivers))]
	[HarmonyPostfix]
	public static void PlayTutelHurtSound(LiveMixin __instance, DamageInfo inDamage)
	{
		if (inDamage.damage == 0) return;
		Pickupable pickupable = __instance.GetComponent<Pickupable>();
		if (!pickupable || !TutelLoader.TutelTechTypes.Contains(pickupable.GetTechType())) return;
		TutelLoader.HurtSounds.Play(__instance.GetComponent<FMOD_CustomEmitter>());
	}

    [HarmonyPatch(typeof(KnownTech), nameof(KnownTech.Initialize))]
    [HarmonyPostfix]
    public static void FixTutelAnalysisTech()
    {
        if (KnownTech.analysisTech is null) return;

        KnownTech.AnalysisTech tech = KnownTech.analysisTech.FirstOrDefault(tech => tech.techType == ModItems.Tutel);
        if (tech is null) return;
        tech.unlockMessage = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredMessage;
        tech.unlockSound = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredSound;
    }
}
