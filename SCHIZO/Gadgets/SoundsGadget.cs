using System;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using SCHIZO.Items;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Gadgets;

public class SoundsGadget : Gadget
{
    public SoundsGadget(ICustomPrefab prefab) : base(prefab)
    {
    }

    public SoundPlayer WorldSoundPlayer { get; set; }
    public SoundPlayer InventorySoundPlayer { get; set; }

    protected override void Build()
    {
        if (prefab is not ItemPrefab customPrefab) throw new InvalidOperationException($"{nameof(SoundsGadget)} can only be applied to an ItemPrefab");
        if (prefab.Info.TechType == TechType.None) throw new InvalidOperationException($"Prefab '{prefab.Info}' must have a TechType.");

        customPrefab.SetPrefabPostProcessor(PrefabPostProcess);
    }

    private void PrefabPostProcess(GameObject prefab)
    {
        if (WorldSoundPlayer != null) WorldSounds.Add(prefab, WorldSoundPlayer);
        if (InventorySoundPlayer != null) InventorySounds.Add(prefab, InventorySoundPlayer);
    }

    public SoundsGadget WithWorldSounds(SoundPlayer player)
    {
        WorldSoundPlayer = player;
        return this;
    }

    public SoundsGadget WithInventorySounds(SoundPlayer player)
    {
        InventorySoundPlayer = player;
        return this;
    }
}

public static class SoundsGadgetExtensions
{
    public static SoundsGadget SetSounds(this ICustomPrefab prefab, SoundPlayer worldSounds = null, SoundPlayer inventorySounds = null)
    {
        if (!prefab.TryGetGadget(out SoundsGadget gadget))
        {
            return prefab.AddGadget(new SoundsGadget(prefab)
            {
                WorldSoundPlayer = worldSounds,
                InventorySoundPlayer = inventorySounds
            });
        }

        if (worldSounds != null) gadget.WorldSoundPlayer = worldSounds;
        if (inventorySounds != null) gadget.InventorySoundPlayer = inventorySounds;
        return gadget;
    }
}
