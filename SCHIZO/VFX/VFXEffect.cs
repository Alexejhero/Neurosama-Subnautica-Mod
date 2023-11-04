using UnityEngine;

namespace SCHIZO.VFX
{
    internal class VFXEffect : MonoBehaviour
    {
        public Material effectMaterial;

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, effectMaterial);
        }
    }
}
