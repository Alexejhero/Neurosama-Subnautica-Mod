namespace SCHIZO.Items;

partial class ItemLoader
{
    public virtual void Load(ModItem modItem)
    {
        new UnityPrefab(modItem).Register();
    }
}
