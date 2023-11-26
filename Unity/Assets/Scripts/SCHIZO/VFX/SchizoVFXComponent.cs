using TriInspector;
using UnityEngine;

namespace SCHIZO.VFX
{

    public class SchizoVFXComponent : MonoBehaviour
    {
        public Effects effect;

        public Texture2D texture;
        public TextureWrapMode wrapMode;

        public Texture2D displacement;
        public TextureWrapMode displacementWrapMode;

        public Color color;
        public Color color2;

        [Range(0f, 1f)]
        public float strength = 1.0f;

        [InfoBox("Blend mode of main image/color (if material supports it)")]
        public BlendMode blendMode;

        private MatPassID mat;

        private void Awake()
        {
            _ = SchizoVFXStack.VFXStack;

            mat = new MatPassID(effect, blendMode);

            if (texture) texture.wrapMode = wrapMode;
            if (displacement) displacement.wrapMode = displacementWrapMode;
        }

        public void Update()
        {
            mat.SetFloat("_Strength", strength);
            mat.SetTexture("_Image", texture);
            mat.SetTexture("_Displacement", displacement);
            mat.SetColor("_Color", color);
            mat.SetColor("_Color0", color2);

            SchizoVFXStack.RenderEffect(mat);
        }
    }
}
