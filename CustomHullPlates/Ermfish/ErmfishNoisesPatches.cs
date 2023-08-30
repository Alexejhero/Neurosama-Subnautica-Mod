using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace SCHIZO.Ermfish;

[HarmonyPatch]
public static class ErmfishNoisesPatches
{
	[HarmonyPatch(typeof(Pickupable), nameof(Pickupable.PlayPickupSound))]
	[HarmonyPostfix]
	public static void PlayErmfishPickupSound(Pickupable __instance)
	{
		if (!ErmfishTypes.All.Contains(__instance.GetTechType())) return;
		ErmfishLoader.PickupSounds.Play();
	}

	[HarmonyPatch(typeof(Pickupable), nameof(Pickupable.PlayDropSound))]
	[HarmonyPostfix]
	public static void PlayErmfishDropSound(Pickupable __instance)
	{
		if (!ErmfishTypes.All.Contains(__instance.GetTechType())) return;
		ErmfishLoader.UnequipSounds.CancelAllDelayed();
		ErmfishLoader.DropSounds.Play(__instance.GetComponent<FMOD_CustomEmitter>());
	}

	[HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.OnDraw))]
	[HarmonyPostfix]
	public static void PlayErmfishDrawSound(PlayerTool __instance)
	{
		if (!ErmfishTypes.All.Contains(__instance.pickupable.GetTechType())) return;
		if (Time.time < ErmfishLoader.PickupSounds.LastPlay + 0.5f) return;
		ErmfishLoader.EquipSounds.Play();
	}

	[HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.OnHolster))]
	[HarmonyPostfix]
	public static void PlayErmfishHolsterSound(PlayerTool __instance)
	{
		if (!ErmfishTypes.All.Contains(__instance.pickupable.GetTechType())) return;
		if (Time.time < ErmfishLoader.DropSounds.LastPlay + 0.5f) return;
		if (Time.time < ErmfishLoader.EatSounds.LastPlay + 0.5f) return;
		if (Time.time < ErmfishLoader.CraftSounds.LastPlay + 0.5f) return;
		ErmfishLoader.UnequipSounds.Play(0.15f);
	}

	[HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Unlock))]
	[HarmonyPostfix]
	public static void PlayErmfishScanSound(PDAScanner.EntryData entryData)
	{
		if (!ErmfishTypes.All.Contains(entryData.key)) return;
		ErmfishLoader.ScanSounds.Play();
	}

	[HarmonyPatch(typeof(CraftData), nameof(CraftData.GetUseEatSound))]
	[HarmonyPostfix]
	public static void PlayErmfishEatSound(TechType techType)
	{
		if (!ErmfishTypes.All.Contains(techType)) return;
		if (Time.time < ErmfishLoader.EatSounds.LastPlay + 0.1f) return;
		ErmfishLoader.UnequipSounds.CancelAllDelayed();
		ErmfishLoader.EatSounds.Play();
	}

	[HarmonyPatch(typeof(Crafter), nameof(Crafter.OnCraftingBegin))]
	[HarmonyPostfix]
	public static void PlayErmfishCookSound(TechType techType)
	{
		if (!ErmfishTypes.All.Contains(techType)) return;
		ErmfishLoader.UnequipSounds.CancelAllDelayed();
		ErmfishLoader.CraftSounds.Play();
	}

	[HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.Kill))]
	[HarmonyPostfix]
	public static void PlayPlayerDeathSound(LiveMixin __instance)
	{
		if (Player.main.liveMixin != __instance) return;
		if (ErmfishTypes.All.All(t => !Inventory.main.container.Contains(t))) return;
		ErmfishLoader.PlayerDeathSounds.Play(0.15f);
	}

	[HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.NotifyAllAttachedDamageReceivers))]
	[HarmonyPostfix]
	public static void PlayErmfishHurtSound(LiveMixin __instance, DamageInfo inDamage)
	{
		if (inDamage.damage == 0) return;
		Pickupable pickupable = __instance.GetComponent<Pickupable>();
		if (!pickupable || !ErmfishTypes.All.Contains(pickupable.GetTechType())) return;
		ErmfishLoader.HurtSounds.Play(__instance.GetComponent<FMOD_CustomEmitter>());
	}

	[HarmonyPatch(typeof(Player), nameof(Player.Update))]
	[HarmonyPostfix]
	public static void PlayErmfishRandomSound()
	{
		foreach (InventoryItem item in ErmfishTypes.All.SelectMany(t => Inventory.main.container.GetItems(t) ?? new List<InventoryItem>()))
		{
			if (!item.item || item.item.gameObject.activeInHierarchy) continue;
			item.item.GetComponent<ErmfishNoises>()?.Update();
		}
	}
}
