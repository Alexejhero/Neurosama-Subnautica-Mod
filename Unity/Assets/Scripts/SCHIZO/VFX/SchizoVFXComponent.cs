using TriInspector;
using UnityEngine;

namespace SCHIZO.VFX
{
    public class SchizoVFXComponent : MonoBehaviour
    {
        [Required ( Message = "Material that is used to render effect on main camera. Always applied if the object, this component is attached to, present in scene.")]
        public Material material;

        [PropertyTooltip(tooltip:"Force each instance of effect in scene to be rendered.")]
        public bool forceUniqueInstance = false;

        private void Awake()
        {
            SchizoVFXStack stack = SchizoVFXStack.VFXStack;
        }

        public void Update()
        {
            if (forceUniqueInstance) { SchizoVFXStack.RenderEffectForceInstance(material); }
            else { SchizoVFXStack.RenderEffect(material); }
           
        }
    }
}
