using System.IO;
using System.Reflection;

namespace SCHIZO;

public static class AssetLoader
{
    public static readonly string AssetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Assets");
}
