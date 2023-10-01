using UnityEngine;

namespace SCHIZO.Creatures;

public abstract class PickupableCreaturePrefab : CustomCreaturePrefab
{
    public ModItem CookedItem { get; }
    public ModItem CuredItem { get; }

    protected PickupableCreaturePrefab(ModItem regular, ModItem cooked, ModItem cured, GameObject creaturePrefab) : base(regular, creaturePrefab)
    {
        CookedItem = cooked;
        CuredItem = cured;
    }
}
