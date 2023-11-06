namespace SCHIZO.Items.Gymbag;

partial class GymbagLoader
{
    public override void Load(ModItem modItem)
    {
        new Gymbag(modItem).Register();
    }
}
