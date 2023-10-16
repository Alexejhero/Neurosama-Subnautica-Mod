namespace SCHIZO.HullPlates;

partial class HullPlateLoader
{
    protected override void Register()
    {
        foreach (HullPlate hullPlate in hullPlates)
        {
            new HullPlatePrefab(hullPlate, this).Register();
        }
    }
}
