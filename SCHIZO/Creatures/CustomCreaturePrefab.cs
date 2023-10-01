using ECCLibrary;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Creatures;

public abstract class CustomCreaturePrefab : CreatureAsset
{
    public ModItem ModItem { get; private set; }

    protected readonly GameObject creaturePrefab;

    protected CustomCreaturePrefab(ModItem modItem, GameObject creaturePrefab) : base(modItem)
    {
        ModItem = modItem;
        this.creaturePrefab = creaturePrefab;
    }

    public new virtual void Register() => base.Register();

    public override void ApplyMaterials(GameObject gameObject) => MaterialHelpers.ApplySNShadersIncludingRemaps(gameObject, 1);
}
