using System.Collections.Generic;
using SCHIZO.Unity.Materials;

namespace SCHIZO.Extensions;

public static class MaterialRemapperExtensions
{
    public static void ApplyAll(this IEnumerable<MaterialRemapper> self, string remapName) => self.ForEach(r => r.ApplyRemap(remapName));
    public static void ApplyAll(this IEnumerable<MaterialRemapper> self, MaterialRemapOverride remap) => self.ForEach(r => r.ApplyRemap(remap.remapName));
}
