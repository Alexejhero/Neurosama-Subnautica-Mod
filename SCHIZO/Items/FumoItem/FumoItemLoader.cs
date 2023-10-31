using Nautilus.Handlers;

namespace SCHIZO.Items.FumoItem;

partial class FumoItemLoader
{
    public override void Load()
    {
        new FumoItem(itemData.ModItem).Register();
        // on ice near the crash site, visible through the pod's windshield
        CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(itemData.ModItem, spawnPosition, spawnRotation));
    }
}
