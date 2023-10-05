using System.Diagnostics.CodeAnalysis;
using ECCLibrary;
using ECCLibrary.Data;
using JetBrains.Annotations;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using SCHIZO.Gadgets;
using SCHIZO.Unity.Materials;

namespace SCHIZO.Creatures;

public sealed class CreatureVariant : CustomPrefab
{
    public bool IsAlive { get; init; } // TODO: Implement variants not being alive
    public Sprite Icon { get; init; }
    public RecipeData RecipeData { get; init; }
    public EdibleData EdibleData { get; init; }
    public string[] FabricatorPath { get; init; }
    public TechCategory TechCategory { [UsedImplicitly] get; init; }
    public MaterialRemapOverride MaterialRemap { get; init; }
    public bool RegisterAsCookedVariant { get; init; }
    public VFXFabricatingData VFXFabricatingData { get; init; }
    public Action<PrefabInfo> PostRegister { get; init; }

    private readonly TechType _original;

    [SetsRequiredMembers]
    public CreatureVariant(TechType original, PrefabInfo variant) : base(variant)
    {
        _original = original;
    }

    public new void Register()
    {
        Info.WithIcon(Icon);

        CraftingGadget crafting = this.SetRecipe(RecipeData);
        crafting.WithFabricatorType(CraftTree.Type.Fabricator);
        crafting.WithStepsToFabricatorTab(FabricatorPath);

        this.SetEdibleData(EdibleData);
        this.SetUnlock(_original);
        this.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);
#if BELOWZERO
        CraftDataHandler.SetSoundType(Info.TechType, TechData.GetSoundType(_original));
#endif

        // in BZ, cooked fish aren't in the PDA
        this.SetPdaGroupCategory(IS_BELOWZERO ? TechGroup.Uncategorized : TechGroup.Survival, TechCategory);

        SetGameObject(new CloneTemplate(Info, _original)
        {
            ModifyPrefab = prefab =>
            {
                // we're not actually applying the remap itself, instead we are applying the Single remap that we find with that name
                // this is so that if we have remappers with different configs, this won't break
                // however i don't think that^ should ever happen anyway
                prefab.GetComponentsInChildren<MaterialRemapper>().ApplyAll(MaterialRemap);
                CreaturePrefabUtils.AddVFXFabricating(prefab, VFXFabricatingData);
            }
        });
        base.Register();

        if (RegisterAsCookedVariant) CraftDataHandler.SetCookedVariant(_original, Info.TechType);

        PostRegister?.Invoke(Info);
    }
}
