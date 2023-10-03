using HarmonyLib;
using Nautilus.Handlers;
using SCHIZO.Attributes;

namespace SCHIZO.Tweaks;

[LoadMethod]
[HarmonyPatch]
public static class SpawnFumoOnNewGame
{
    [LoadMethod]
    public static void RegisterSpawn()
    {
        LOGGER.LogWarning("Registering fumo spawn");
        CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(ModItems.NeuroFumoItem, new(-307, 18f, 274f)));
    }
}
