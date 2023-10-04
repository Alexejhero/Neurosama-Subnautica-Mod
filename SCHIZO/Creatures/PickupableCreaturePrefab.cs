using System;
using ECCLibrary.Data;
using UnityEngine;

namespace SCHIZO.Creatures;

public abstract class PickupableCreaturePrefab<TCreature> : CustomCreaturePrefab<TCreature>
    where TCreature : Creature
{
    private readonly EdibleData _edibleData;
    private readonly float _bioReactorCharge;

    #region ECCLibrary properties

    /// <summary>
    /// Contains data pertaining to picking up and/or holding fish in your hands. Not assigned by default.
    /// </summary>
    protected PickupableFishData PickupableFishData { get; init; }

    #endregion

    protected PickupableCreaturePrefab(ModItem modItem, GameObject rawObject, CreatureVariantType type, IPickupableCreatureLoader loader) : base(modItem, rawObject)
    {


        _bioReactorCharge = loader.BioReactorCharge;
    }

    public override CreatureTemplate CreateTemplate()
    {
        CreatureTemplate template = base.CreateTemplate();
        template.PickupableFishData = PickupableFishData;
        template.EdibleData = _edibleData;
        template.BioReactorCharge = _bioReactorCharge;
        return template;
    }
}
