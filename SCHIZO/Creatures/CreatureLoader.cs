using SCHIZO.Items;

namespace SCHIZO.Creatures;

partial class CreatureLoader
{
    public override void Load(ModItem modItem)
    {
        new UnityCreaturePrefab(modItem).Register();
    }
}
