using System;
using System.Collections.Generic;
using Nautilus.Utility;

namespace SCHIZO.Sounds;

partial class ItemSounds
{
    private static readonly Dictionary<TechType, ItemSounds> _registeredItemSounds = new();

    public static bool TryGet(TechType techType, out ItemSounds itemSounds)
    {
        return _registeredItemSounds.TryGetValue(techType, out itemSounds);
    }

    public void Register(TechType techType)
    {
        if (_registeredItemSounds.TryGetValue(techType, out ItemSounds existingItemSounds) && existingItemSounds != this)
        {
            throw new Exception($"ItemSounds for {techType} already registered by {existingItemSounds}");
        }

        _registeredItemSounds[techType] = this;

        // If multiple items share the same sounds, Initialize will return the same SoundCollectionInstance as long as the bus is the same, so we don't need to worry about not calling it.

        pickupSounds = pickupSounds!?.Initialize(AudioUtils.BusPaths.PDAVoice);
        dropSounds = dropSounds!?.Initialize(AudioUtils.BusPaths.UnderwaterCreatures);

        drawSounds = drawSounds!?.Initialize(AudioUtils.BusPaths.PDAVoice);
        holsterSounds = holsterSounds!?.Initialize(AudioUtils.BusPaths.PDAVoice);

        cookSounds = cookSounds!?.Initialize(AudioUtils.BusPaths.PDAVoice);
        eatSounds = eatSounds!?.Initialize(AudioUtils.BusPaths.PDAVoice);
    }
}
