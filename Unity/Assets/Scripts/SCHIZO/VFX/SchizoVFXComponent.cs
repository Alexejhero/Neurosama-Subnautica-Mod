using UnityEngine;

namespace SCHIZO.VFX
{
    public sealed partial class SchizoVFXComponent : MonoBehaviour
    {
        public Material effectMaterial;
        private int id = Shader.PropertyToID("_ScreenPosition");

        private void Update()
        {
            Vector3 screenpos = Camera.main.WorldToScreenPoint(transform.position);
            effectMaterial.SetVector(id, new Vector4(screenpos.x, screenpos.y, screenpos.z, 0));
        }
    }
}
