using UnityEngine;

namespace SCHIZO.SchizoVFX
{
    public partial class SchizoVFXComponent : MonoBehaviour
    {
        public Material effectMaterial;
        private int id = Shader.PropertyToID("_ScreenPosition");

        private void Update()
        {
            Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
            effectMaterial.SetVector(id, new Vector4(pos.x, pos.y, 0f, 0f));
        }
    }
}
