using UnityEngine;
using UWE;

namespace SCHIZO.Effects_test
{
    public partial class SchizoEffect
    {
        ImageEffectWithEvents eff;
        public void Awake()
        {
            eff = Camera.main.gameObject.GetComponent<ImageEffectWithEvents>();
            eff.CheckShaderAndCreateMaterial(shader,material);
            eff.afterOnRenderImage += effect;
        }

        private void effect(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, material);
        }
    }
}
