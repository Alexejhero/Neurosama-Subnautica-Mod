using System;
using ECCLibrary.Data;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using UnityEngine;

namespace SCHIZO.API.Gadgets;

public sealed class EatableGadget : Gadget
{
    public float FoodValue { get; set; }
    public float WaterValue { get; set; }
    public bool Decomposes { get; set; }
    public float DecayRate { get; set; }

    public EatableGadget(ICustomPrefab prefab) : base(prefab)
    {
    }

    public EatableGadget WithNutritionValues(float foodValue, float waterValue)
    {
        FoodValue = foodValue;
        WaterValue = waterValue;
        return this;
    }

    public EatableGadget WithDecay(float decayRate)
    {
        Decomposes = true;
        DecayRate = decayRate;
        return this;
    }

    public EatableGadget WithDecay(bool decomposes)
    {
        Decomposes = decomposes;
        DecayRate = 1;
        return this;
    }

    protected override void Build()
    {
        if (prefab is not CustomPrefab customPrefab) throw new InvalidOperationException($"{nameof(EatableGadget)} can only be applied to a CustomPrefab.");
        if (prefab.Info.TechType == TechType.None) throw new InvalidOperationException($"Prefab '{prefab.Info}' must have a TechType.");

        customPrefab.SetPrefabPostProcessor(PrefabPostProcess);
    }

    private void PrefabPostProcess(GameObject obj)
    {
        Eatable eatable = obj.EnsureComponent<Eatable>();
        eatable.foodValue = FoodValue;
        eatable.waterValue = WaterValue;
        eatable.kDecayRate = DecayRate * 0.015f;
        eatable.decomposes = Decomposes;
    }
}

public static class EatableGadgetExtensions
{
    public static EatableGadget SetNutritionValues(this CustomPrefab prefab, float foodValue, float waterValue)
    {
        if (!prefab.TryGetGadget(out EatableGadget gadget))
            gadget = prefab.AddGadget(new EatableGadget(prefab));

        gadget.FoodValue = foodValue;
        gadget.WaterValue = waterValue;
        return gadget;
    }

    public static EatableGadget SetEdibleData(this CustomPrefab prefab, EdibleData data)
    {
        if (!prefab.TryGetGadget(out EatableGadget gadget))
            gadget = prefab.AddGadget(new EatableGadget(prefab));

        gadget.FoodValue = data.foodAmount;
        gadget.WaterValue = data.waterAmount;
        gadget.Decomposes = data.decomposes;
        gadget.DecayRate = data.decomposeSpeed;
        return gadget;
    }
}
