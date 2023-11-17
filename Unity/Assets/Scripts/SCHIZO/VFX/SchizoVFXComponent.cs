using TriInspector;
using UnityEngine;

namespace SCHIZO.VFX
{
    public class SchizoVFXComponent : MonoBehaviour
    {
        [Required(Message = "Material that is used to render effect on main camera. Always applied if the object, this component is attached to, present in scene.")]
        public Material material;

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

        [PropertyTooltip(tooltip: "Force each instance of effect in scene to be rendered.")]
        public bool forceUniqueInstanceOnClones = false;

        private MatPassID mat;

        private void Awake()
        {
            mat = new MatPassID(forceUniqueInstanceOnClones ? new Material(material) : material, blendMode);

            if (texture) texture.wrapMode = wrapMode;
            if (displacement) displacement.wrapMode = displacementWrapMode;

            mat.mat.SetFloat("_Strength", strength);
            mat.mat.SetTexture("_Image", texture);
            mat.mat.SetTexture("_Displacement", displacement);
            mat.mat.color = color;
            mat.mat.SetColor("_Color0", color2);

            _ = SchizoVFXStack.VFXStack;
        }

        public void Update()
        {
            SchizoVFXStack.RenderEffect(mat);
        }
    }
}
