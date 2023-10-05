using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCHIZO.Unity.Materials;

public static class MaterialRemapperExtensions
{
    public static void ApplyAll(this IEnumerable<MaterialRemapper> self, string remapName) => self.ForEach(r => r.ApplyRemap(remapName));
    public static void ApplyAll(this IEnumerable<MaterialRemapper> self, MaterialRemapOverride remap) => self.ForEach(r => r.ApplyRemap(remap.remapName));
}
