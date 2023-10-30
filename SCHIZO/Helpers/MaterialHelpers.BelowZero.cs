using System.Collections;
using System.Runtime.CompilerServices;
using SCHIZO.Attributes.Loading;
using UnityEngine;

namespace SCHIZO.Helpers;

[LoadMethod]
public static partial class MaterialHelpers
{
    private static Material _ghostMaterial;
    private static Texture _constrEmissiveTex;
    private static Texture _constrNoiseTex;

    public static Material GhostMaterial => Check(_ghostMaterial);
    public static Texture ConstructableEmissiveTexture => Check(_constrEmissiveTex);
    public static Texture ConstructableNoiseTexture => Check(_constrNoiseTex);

    private static bool _isReady;

    private static T Check<T>(T item, [CallerMemberName] string memberName = null)
    {
        if (!_isReady) LOGGER.LogFatal($"Materials are not ready yet, so {nameof(MaterialHelpers)}.{memberName} will return null!");
        return item;
    }

    public static IEnumerator LoadMaterials()
    {
        yield return LoadGhostMaterial();
        _isReady = true;
    }

    private static IEnumerator LoadGhostMaterial()
    {
        if (_ghostMaterial)
            yield break;

        CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.StarshipChair);
        yield return task;

        if (task.GetResult() is not GameObject chair)
        {
            LOGGER.LogFatal("Couldn't load ghost material - no prefab");
            yield break;
        }

        Constructable con = chair.GetComponentInChildren<Constructable>();
        _ghostMaterial = con.ghostMaterial;
        _constrEmissiveTex = con._EmissiveTex;
        _constrNoiseTex = con._NoiseTex;
    }
}
