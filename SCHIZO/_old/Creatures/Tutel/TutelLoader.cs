using System.Collections.Generic;
using ECCLibrary.Data;
using Nautilus.Handlers;
using SCHIZO.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.Unity.Creatures;
using UnityEngine;

namespace SCHIZO.Creatures.Tutel;

[LoadCreature]
public sealed class TutelLoader : PickupableCreatureLoader<PickupableCreatureData, TutelPrefab, TutelLoader>
{
    protected override void PostRegisterAlive(ModItem item)
    {
        base.PostRegisterAlive(item);
        ItemActionHandler.RegisterMiddleClickAction(item, _ => creatureSounds.AmbientItemSounds.Play2D(5), "ping @vedal987", "English");
    }
}
