using System.Linq;
using HarmonyLib;
using Nautilus.Handlers;
using UnityEngine;

namespace SCHIZO.Creatures.Ermfish;

[HarmonyPatch]
public static class ErmfishPatches
{
	[HarmonyPatch(typeof(Pickupable), nameof(Pickupable.PlayPickupSound))]
	[HarmonyPostfix]
	public static void PlayErmfishPickupSound(Pickupable __instance)
	{
		if (!ErmfishLoader.ErmfishTechTypes.Contains(__instance.GetTechType())) return;
		ErmfishLoader.PickupSounds.Play2D();
	}

	[HarmonyPatch(typeof(Pickupable), nameof(Pickupable.PlayDropSound))]
	[HarmonyPostfix]
	public static void PlayErmfishDropSound(Pickupable __instance)
	{
		if (!ErmfishLoader.ErmfishTechTypes.Contains(__instance.GetTechType())) return;
		ErmfishLoader.UnequipSounds.CancelAllDelayed();
		ErmfishLoader.DropSounds.Play(__instance.GetComponent<FMOD_CustomEmitter>());
	}

	[HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.OnDraw))]
	[HarmonyPostfix]
	public static void PlayErmfishDrawSound(PlayerTool __instance)
	{
		try
		{
			if (!__instance.pickupable || !ErmfishLoader.ErmfishTechTypes.Contains(__instance.pickupable.GetTechType())) return;
			if (Time.time < ErmfishLoader.PickupSounds.LastPlay + 0.5f) return;
			ErmfishLoader.EquipSounds.Play2D();
		}
		catch
		{
			// ignore
		}
	}

	[HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.OnHolster))]
	[HarmonyPostfix]
	public static void PlayErmfishHolsterSound(PlayerTool __instance)
	{
		try
		{
			if (!__instance.pickupable || !ErmfishLoader.ErmfishTechTypes.Contains(__instance.pickupable.GetTechType())) return;
			if (Time.time < ErmfishLoader.DropSounds.LastPlay + 0.5f) return;
			if (Time.time < ErmfishLoader.EatSounds.LastPlay + 0.5f) return;
			if (Time.time < ErmfishLoader.CraftSounds.LastPlay + 0.5f) return;
			ErmfishLoader.UnequipSounds.Play2D(0.15f);
		}
		catch
		{
			// ignore
		}
	}

	[HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Unlock))]
	[HarmonyPostfix]
	public static void PlayErmfishScanSound(PDAScanner.EntryData entryData)
	{
		if (!ErmfishLoader.ErmfishTechTypes.Contains(entryData.key)) return;
		ErmfishLoader.ScanSounds.Play2D();
	}

	[HarmonyPatch(typeof(CraftData), nameof(CraftData.GetUseEatSound))]
	[HarmonyPostfix]
	public static void PlayErmfishEatSound(TechType techType)
	{
		if (!ErmfishLoader.ErmfishTechTypes.Contains(techType)) return;
		if (Time.time < ErmfishLoader.EatSounds.LastPlay + 0.1f) return;
		ErmfishLoader.UnequipSounds.CancelAllDelayed();
		ErmfishLoader.EatSounds.Play2D();
	}

	[HarmonyPatch(typeof(Crafter), nameof(Crafter.OnCraftingBegin))]
	[HarmonyPostfix]
	public static void PlayErmfishCookSound(TechType techType)
	{
		if (!ErmfishLoader.ErmfishTechTypes.Contains(techType)) return;
		ErmfishLoader.UnequipSounds.CancelAllDelayed();
		ErmfishLoader.CraftSounds.Play2D();
	}

	[HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.Kill))]
	[HarmonyPostfix]
	public static void PlayPlayerDeathSound(LiveMixin __instance)
	{
		if (Player.main.liveMixin != __instance) return;
		if (ErmfishLoader.ErmfishTechTypes.All(t => !Inventory.main.container.Contains(t))) return;
		ErmfishLoader.PlayerDeathSounds.Play2D(0.15f);
	}

	[HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.NotifyAllAttachedDamageReceivers))]
	[HarmonyPostfix]
	public static void PlayErmfishHurtSound(LiveMixin __instance, DamageInfo inDamage)
	{
		if (inDamage.damage == 0) return;
		Pickupable pickupable = __instance.GetComponent<Pickupable>();
		if (!pickupable || !ErmfishLoader.ErmfishTechTypes.Contains(pickupable.GetTechType())) return;
		ErmfishLoader.HurtSounds.Play(__instance.GetComponent<FMOD_CustomEmitter>());
	}

    [HarmonyPatch(typeof(KnownTech), nameof(KnownTech.Initialize))]
    [HarmonyPostfix]
    public static void FixErmfishAnalysisTech()
    {
        if (KnownTech.analysisTech is null) return;

        KnownTech.AnalysisTech tech = KnownTech.analysisTech.FirstOrDefault(tech => tech.techType == ModItems.Ermfish);
        if (tech is null) return;
        tech.unlockMessage = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredMessage;
        tech.unlockSound = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredSound;
    }
}
