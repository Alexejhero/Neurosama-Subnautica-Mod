using System;
using System.Collections.Generic;

namespace SCHIZO.Sounds;

public static class CreatureSoundsHandler
{
    private static readonly Dictionary<TechType, CreatureSounds> RegisteredSounds = new();

    public static void RegisterCreatureSounds(TechType techType, CreatureSounds sounds)
    {
        RegisteredSounds[techType] = sounds ?? throw new ArgumentNullException(nameof(sounds));
    }

    public static bool TryGetCreatureSounds(TechType techType, out CreatureSounds sounds)
        => RegisteredSounds.TryGetValue(techType, out sounds);

    public static CreatureSounds GetCreatureSounds(TechType techType) => RegisteredSounds[techType];
}
