using TriInspector;
using UnityEngine;

namespace SCHIZO.VFX
{
    public partial class SchizoVFXComponent : MonoBehaviour
    {
        [Required ( Message = "Material that is used to render effect on main camera. Always applied if the object, this component is attached to, present in scene.")]
        public Material material;

        [HideInInspector]
        public bool usingCustomMaterial = false;

        public void Awake()
        {
            if ( Camera.main.GetComponent<SchizoVFXStack>() == null)  Camera.main.gameObject.AddComponent<SchizoVFXStack>();
        }

        public void Update()
        {
            if (!usingCustomMaterial)
            {
                SchizoVFXStack.effectMaterials.Add(material);
            }
        }

        public void applyEffect(Material material)
        {
            SchizoVFXStack.effectMaterials.Add(material);
        }
    }
}
