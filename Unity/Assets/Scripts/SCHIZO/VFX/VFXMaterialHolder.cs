using TriInspector;
using UnityEngine;

namespace SCHIZO.VFX
{
    public enum Effects
    {
        ColorTint,
        TwoColorTint,
        NoiseVignette,
        ImageOverlay,
        ARGCensor,
    }

    public class VFXMaterialHolder : MonoBehaviour
    {
        public static VFXMaterialHolder instance { get; private set; }

        public static Texture2DArray t2dArrayForARGEffect; 

        [Required]
        public Material ColorTintMaterial;

        [Required]
        public Material TwoColorTintMaterial;

        [Required]
        public Material NoiseVignetteMaterial;

        [Required]
        public Material ImageOverlayMaterial;

        [Required]
        public Material ARGCensorMaterial;

        public Material GetMaterialForEffect(Effects effect)
        {
            switch (effect)
            {
                case Effects.ColorTint:
                    return ColorTintMaterial;
                case Effects.TwoColorTint:
                    return TwoColorTintMaterial;
                case Effects.NoiseVignette:
                    return NoiseVignetteMaterial;
                case Effects.ImageOverlay:
                    return ImageOverlayMaterial;
                case Effects.ARGCensor:
                    return ARGCensorMaterial;
            }
            return ColorTintMaterial;
        }

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }
    }
}
