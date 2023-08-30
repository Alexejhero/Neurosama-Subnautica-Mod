using Nautilus.Assets;

namespace SCHIZO.Ermshark;

public static class ErmsharkLoader
{
    public static void Load()
    {
        new Ermshark(PrefabInfo.WithTechType("ermshark", "Ermshark", "erm")).Register();
    }
}
