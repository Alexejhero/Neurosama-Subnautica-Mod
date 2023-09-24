using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Nautilus.Utility;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO;

public static class AssetLoader
{
    public static readonly string AssetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Assets");
}
