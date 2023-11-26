namespace SCHIZO.Items.ErmBed;

partial class ErmBedLoader
{
    public override void Load(ModItem modItem)
    {
        new ErmBed(modItem).Register();
    }
}
