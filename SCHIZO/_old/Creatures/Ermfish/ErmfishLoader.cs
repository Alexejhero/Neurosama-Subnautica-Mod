using System.Collections.Generic;
using ECCLibrary.Data;
using Nautilus.Handlers;
using SCHIZO.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.Sounds;
using SCHIZO.Unity.Creatures;
using UnityEngine;

namespace SCHIZO.Creatures.Ermfish;

[LoadCreature]
public sealed class ErmfishLoader : PickupableCreatureLoader<PickupableCreatureData, ErmfishPrefab, ErmfishLoader>
{
    // todo: test BZ bus path when SoundPlayers are fixed
    public static readonly SoundPlayer PlayerDeathSounds = new(Assets.Erm_Sounds_PlayerDeath_ErmfishPlayerDeath,
        IS_BELOWZERO ? "bus:/master/SFX_for_pause" : "bus:/master/SFX_for_pause/nofilter");
}
