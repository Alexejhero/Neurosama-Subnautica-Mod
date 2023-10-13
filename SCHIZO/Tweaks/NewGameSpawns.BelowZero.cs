using Nautilus.Handlers;
using SCHIZO.Attributes;
using SCHIZO.Resources;

namespace SCHIZO.Tweaks;

[LoadMethod]
public static class NewGameSpawns
{
    [LoadMethod]
    public static void RegisterNewGameSpawns()
    {
        // on a rock near the crash site, should be visible at the end of the intro cutscene
        CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(Assets.Neurofumos_Neurofumo_NeurofumoItemData.ModItem, new(-307, 18f, 274f)));
    }
}
