using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace SCHIZO.VFX
{
    public class VFXMaterialHolder : MonoBehaviour
    {
        public static VFXMaterialHolder instance { get; private set; }
        [InlineEditor]
        public List<VFXMaterial> materials;
        public Material GetMaterial(string materialName)
        {
            return materials.Find(x => x.materialName == materialName).material;
        }

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }
    }
}
