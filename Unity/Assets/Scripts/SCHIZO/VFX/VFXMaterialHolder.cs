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

        [Required, SerializeField]
        private Material _ColorTintMaterial;

        [Required, SerializeField]
        private Material _TwoColorTintMaterial;

        [Required, SerializeField]
        private Material _NoiseVignetteMaterial;

        [Required, SerializeField]
        private Material _ImageOverlayMaterial;

        [Required, SerializeField]
        private Material _ARGCensorMaterial;

        public Material GetMaterialForEffect(Effects effect)
        {
            switch (effect)
            {
                case Effects.ColorTint:
                    return _ColorTintMaterial;
                case Effects.TwoColorTint:
                    return _TwoColorTintMaterial;
                case Effects.NoiseVignette:
                    return _NoiseVignetteMaterial;
                case Effects.ImageOverlay:
                    return _ImageOverlayMaterial;
                case Effects.ARGCensor:
                    return _ARGCensorMaterial;
            }
            return _ColorTintMaterial;
        }

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }
    }
}
