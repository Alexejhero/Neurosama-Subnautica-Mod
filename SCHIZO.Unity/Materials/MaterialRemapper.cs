using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Materials
{
    public sealed partial class MaterialRemapper : MonoBehaviour
    {
        public MaterialRemapConfig config;

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
}
