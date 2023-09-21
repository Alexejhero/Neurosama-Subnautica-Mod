#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
// ReSharper disable ArrangeNamespaceBody

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCHIZO.Unity
{
    public sealed class MaterialRemapper : MonoBehaviour
    {
        [SerializeField] private List<Material> original;
        [SerializeField] private List<Remapping> remappings;

        public void ApplyRemap(string remapName)
        {
            Remapping remapping = remappings.Single(r => r.name == remapName);
            Dictionary<Material, Material> remapDict = GetRemapDictionary(remapping);

            foreach (Renderer rend in GetComponentsInChildren<Renderer>(true))
            {
                for (int i = 0; i < rend.materials.Length; i++)
                {
                    if (remapDict.TryGetValue(rend.materials[i], out Material replacement))
                        rend.materials[i] = replacement;
                }
            }
        }

        private Dictionary<Material, Material> GetRemapDictionary(Remapping remapping)
        {
            IEnumerable<(Material o, Material n)> zipped = Enumerable.Zip(original, remapping.materials, (o, n) => (o, n));
            return zipped.ToDictionary(e => e.o, e => e.n);
        }

        [Serializable]
        private sealed class Remapping
        {
            public string name;
            public List<Material> materials;
        }
    }

#if !UNITY_EDITOR
    public static class MaterialRemapperExtensions
    {
        public static void ApplyAll(this IEnumerable<MaterialRemapper> self, string remapName) => self.ForEach(r => r.ApplyRemap(remapName));
    }
#endif
}
