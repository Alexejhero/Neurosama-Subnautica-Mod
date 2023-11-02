using UnityEngine;
using UWE;

namespace SCHIZO.SchizoVFX
{
    public partial class SchizoVFXComponent
    {
        public void Awake()
        {
            ImageEffectWithEvents eff = Camera.main.gameObject.GetComponent<ImageEffectWithEvents>();
            eff.afterOnRenderImage += DoEffect;
        }
        private void DoEffect(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, effectMaterial);
        }
    }
}
