using Nautilus.Assets;
using SCHIZO.Ermfish;

namespace SCHIZO.Ermshark;

public static class ErmsharkLoader
{
    public static void Load()
    {
        new ErmsharkPrefab(ErmsharkData.Info).Register();
    }
}
