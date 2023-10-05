using ECCLibrary.Data;

namespace SCHIZO.Creatures;

public abstract class PickupableCreaturePrefab<TCreature> : CustomCreaturePrefab<TCreature>, IPickupableCreaturePrefab
    where TCreature : Creature
{
    public ModItem CookedItem { get; }
    public ModItem CuredItem { get; }

    public float FoodValueRaw { get; protected init; } = 10;
    public float WaterValueRaw { get; protected init; } = -10;

    public float FoodValueCooked { get; protected init; } = 20;
    public float WaterValueCooked { get; protected init; } = 5;

    #region ECCLibrary properties

    /// <summary>
    /// Contains data pertaining to picking up and/or holding fish in your hands. Not assigned by default.
    /// </summary>
    protected PickupableFishData PickupableFishData { get; init; }

    /// <summary>
    /// Total power output of this creature. All ECC creatures can be put in the bioreactor as long as this value is greater than 0. Default value is 200.
    /// </summary>
    protected float BioReactorCharge { get; init; } = 200f;

    #endregion

    protected PickupableCreaturePrefab(ModItem regular, ModItem cooked, ModItem cured, GameObject creaturePrefab) : base(regular, creaturePrefab)
    {
        CookedItem = cooked;
        CuredItem = cured;
    }

    public override CreatureTemplate CreateTemplate()
    {
        CreatureTemplate template = base.CreateTemplate();
        template.PickupableFishData = PickupableFishData;
        template.EdibleData = new EdibleData(FoodValueRaw, WaterValueRaw, false, 1);
        template.BioReactorCharge = BioReactorCharge;
        return template;
    }
}

public interface IPickupableCreaturePrefab : ICustomCreaturePrefab
{
    ModItem CookedItem { get; }
    ModItem CuredItem { get; }

    float FoodValueRaw { get; }
    float FoodValueCooked { get; }

    float WaterValueRaw { get; }
    float WaterValueCooked { get; }
}
