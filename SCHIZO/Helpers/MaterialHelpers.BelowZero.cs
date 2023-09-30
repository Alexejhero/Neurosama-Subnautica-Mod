using System.Collections;
using System.Runtime.CompilerServices;
using SCHIZO.Attributes;
using UnityEngine;
using UWE;

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

    public static bool IsReady { get; private set; }

    private static T Check<T>(T item, [CallerMemberName]string memberName = null)
    {
        if (!IsReady) LOGGER.LogWarning($"Materials are not ready yet, so {memberName} might return null! Start a coroutine to wait until {nameof(MaterialHelpers)}.{nameof(IsReady)} is true.");
        return item;
    }

    [LoadMethod]
    public static void LoadMaterials()
    {
        CoroutineHost.StartCoroutine(LoadMaterialsCoro());
    }

    private static IEnumerator LoadMaterialsCoro()
    {
        yield return LoadGhostMaterial();
        IsReady = true;
    }

    private static IEnumerator LoadGhostMaterial()
    {
        if (_ghostMaterial)
            yield break;

        IPrefabRequest task = PrefabDatabase.GetPrefabAsync("26cdb865-efbd-403c-8873-92453bcfc935");
        yield return task;

        if (task.TryGetPrefab(out GameObject chair))
        {
            Constructable con = chair.GetComponentInChildren<Constructable>();
            _ghostMaterial = con.ghostMaterial;
            _constrEmissiveTex = con._EmissiveTex;
            _constrNoiseTex = con._NoiseTex;
        }
        else
        {
            LOGGER.LogError("Couldn't load ghost material - no prefab");
        }
    }

    public static void FixBZGhostMaterial(Constructable con)
    {
        CoroutineHelpers.RunWhen(() =>
        {
            con.ghostMaterial = GhostMaterial;
            con._EmissiveTex = ConstructableEmissiveTexture;
            con._NoiseTex = ConstructableNoiseTexture;
        }, () => IsReady);
    }
}
