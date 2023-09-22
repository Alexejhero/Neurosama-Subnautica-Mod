#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace SCHIZO.Unity.Materials
{
    public sealed class MaterialRemapper : MonoBehaviour
    {
        public MaterialRemapConfig config;

        [UsedImplicitly]
        public void ApplyRemap(string remapName)
        {
            MaterialRemapOverride materialRemapOverride = config.remappings.Single(r => r.remapName == remapName);
            Dictionary<(Texture, Color), Material> remapDict = GetRemapDictionary(materialRemapOverride);

            foreach (Renderer rend in GetComponentsInChildren<Renderer>(true))
            {
                Material[] mats = rend.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    if (remapDict.TryGetValue((mats[i].mainTexture, mats[i].color), out Material replacement))
                    {
                        mats[i] = replacement;
                    }
                }
                rend.materials = mats;
            }
        }

        private Dictionary<(Texture, Color), Material> GetRemapDictionary(MaterialRemapOverride materialRemapOverride)
        {
            IEnumerable<(Material o, Material n)> zipped = Enumerable.Zip(config.original, materialRemapOverride.materials, (o, n) => (o, n));
            return zipped.ToDictionary(e => (e.o.mainTexture, e.o.color), e => e.n);
        }
    }

#if !UNITY_2019_4_OR_NEWER
    public static class MaterialRemapperExtensions
    {
        public static void ApplyAll(this IEnumerable<MaterialRemapper> self, string remapName) => self.ForEach(r => r.ApplyRemap(remapName));
        public static void ApplyAll(this IEnumerable<MaterialRemapper> self, MaterialRemapOverride remap) => self.ForEach(r => r.ApplyRemap(remap.remapName));
    }
#endif
}
