using Nautilus.Handlers;
using SCHIZO.Attributes;

namespace SCHIZO.Tweaks;

[LoadMethod]
public static class NewGameSpawns
{
    [LoadMethod]
    public static void RegisterNewGameSpawns()
    {
        // on a rock near the crash site, should be visible at the end of the intro cutscene
        CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(ModItems.NeuroFumoItem, new(-307, 18f, 274f)));
    }
}
