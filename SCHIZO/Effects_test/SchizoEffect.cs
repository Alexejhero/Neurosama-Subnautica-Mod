using UnityEngine;
using UWE;

namespace SCHIZO.Effects_test
{
    public partial class SchizoEffect
    {
        public void Awake()
        {
            ImageEffectWithEvents eff = Camera.main.gameObject.GetComponent<ImageEffectWithEvents>();
            eff.afterOnRenderImage += DoEffect;
        }

        private void Update()
        {
            Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
            material.SetVector(0, new Vector4(pos.x, pos.y, 0, 0));
        }
        private void DoEffect(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, material);
        }
    }
}
