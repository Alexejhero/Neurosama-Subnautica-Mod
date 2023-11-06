using System;
using System.Collections.Generic;

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
    }
}
