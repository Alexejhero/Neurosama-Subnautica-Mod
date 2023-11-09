using TriInspector;
using UnityEngine;

namespace SCHIZO.VFX
{
    public class SchizoVFXComponent : MonoBehaviour
    {
        [Required ( Message = "Material that is used to render effect on main camera. Always applied if the object, this component is attached to, present in scene.")]
        public Material material;


        public void Update()
        {
            SchizoVFXStack.effectMaterials.Add(material);
        }
    }
}
