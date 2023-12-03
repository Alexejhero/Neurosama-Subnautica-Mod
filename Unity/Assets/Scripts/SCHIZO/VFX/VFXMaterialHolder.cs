using SCHIZO.Utilities;
using TriInspector;
using UnityEngine;

namespace SCHIZO.VFX
{
    public enum Effects
    {
        /// <summary>
        /// Requires one color "_Color" where alpha is transparency. Supports <see cref ="BlendMode"></see> blend modes.
        /// </summary>
        ColorTint,

        /// <summary>
        /// Requires two colors: "_Color" - inner color and "_Color0" - outer color.
        /// </summary>
        TwoColorTint,

        /// <summary>
        /// Displays texture "_Image" in center of the screen with float scale "_Scale", where "_Strength" is transparency in 0 - 1 range.
        /// Supports <see cref ="BlendMode"></see> blend modes.
        /// </summary>
        ImageOverlay,

        /// <summary>
        /// Scrolls texture "_Image" and normal texture "_Displacement", to create an effect of animated noise, enhanced by displacing image on screen,
        /// where float "_DispStrength" is strength of displacement from normal texture in range 0 - 1, and float "_Strength" is overall transparency of the effect in 0 - 1 range.
        /// </summary>
        NoiseVignette,

        /// <summary>
        /// Displays texture from "_Images" Texture2DArray at index provided in "_ScreenPosition" Vector's fourth component, at screen position "_ScreenPosition",
        /// where first three components are respective components of a return vector from <c> Camera.main.WorldToScreenPoint() </c> ,
        /// with float scale "_Scale" and float opacity "_Strength" in 0 - 1 range. Scale is decreasing by distance with formula: (1 / _Scale) * _ScreenPosition.z;
        /// </summary>
        ARGCensor,
    }

    public class VFXMaterialHolder : MonoBehaviour
    {
        public static VFXMaterialHolder instance { get; private set; }

        public Texture2DArray t2dArrayForARGEffect;

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
            return effect switch
            {
                Effects.ColorTint => ColorTintMaterial,
                Effects.TwoColorTint => TwoColorTintMaterial,
                Effects.NoiseVignette => NoiseVignetteMaterial,
                Effects.ImageOverlay => ImageOverlayMaterial,
                Effects.ARGCensor => ARGCensorMaterial,
                _ => ColorTintMaterial,
            };
        }

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }
    }
}
