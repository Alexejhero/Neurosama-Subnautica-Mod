using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SCHIZO.Extensions;

namespace SCHIZO.Resources;

public static class DependencyResolver
{
    public static void InjectResources()
    {
        LOGGER.LogInfo("Added assembly resolve hook");
        AppDomain.CurrentDomain.AssemblyResolve += ResolveDependencies;
    }

    private static Assembly ResolveDependencies(object _, ResolveEventArgs args)
    {
        string name = new AssemblyName(args.Name).Name;

        LOGGER.LogDebug($"Trying to resolve {name} from resources");

        IEnumerable<string> resources = Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(s => s.EndsWith(name + ".dll"));
        string resourceName = resources.FirstOrDefault();
        if (resourceName == null) return null;

        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream == null) return null;

        LOGGER.LogInfo($"Loading assembly {resourceName} from resources");

        return Assembly.Load(stream.ReadFully());
    }
}
