using System.Runtime.CompilerServices;
using SCHIZO.Attributes;
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

    private static T Check<T>(T item, [CallerMemberName] string memberName = null)
    {
        if (!IsReady) LOGGER.LogWarning($"Materials are not ready yet, so {memberName} might return null! Start a coroutine to wait until {nameof(MaterialHelpers)}.{nameof(IsReady)} is true.");
        return item;
    }

    [LoadMethod]
    private static void LoadMaterials()
    {
        CoroutineHost.StartCoroutine(LoadMaterialsCoro());
        return;

        static IEnumerator LoadMaterialsCoro()
        {
            yield return LoadGhostMaterial();
            IsReady = true;
        }
    }

    private static IEnumerator LoadGhostMaterial()
    {
        if (_ghostMaterial)
            yield break;

        CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.StarshipChair);
        yield return task;

        if (task.GetResult() is not GameObject chair)
        {
            LOGGER.LogError("Couldn't load ghost material - no prefab");
            yield break;
        }

        Constructable con = chair.GetComponentInChildren<Constructable>();
        _ghostMaterial = con.ghostMaterial;
        _constrEmissiveTex = con._EmissiveTex;
        _constrNoiseTex = con._NoiseTex;
    }

    public static partial void FixBZGhostMaterial(Constructable con)
    {
        CoroutineHelpers.RunWhen(() =>
        {
            con.ghostMaterial = GhostMaterial;
            con._EmissiveTex = ConstructableEmissiveTexture;
            con._NoiseTex = ConstructableNoiseTexture;
        }, () => IsReady);
    }
}
