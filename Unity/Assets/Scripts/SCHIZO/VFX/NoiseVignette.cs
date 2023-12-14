using TriInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SCHIZO.VFX
{
    [AddComponentMenu("SCHIZO/VFX/Noise Vignette")]
    public class NoiseVignette : VFXComponent
    {
        public Texture2D noiseTexture;

        [ValidateInput(nameof(ValidateNormalMap))]
        public Texture2D displacementNormal;

        [Range(0f, 1f)] public float displacementStrength = 0.5f;
        [Range(0f, 1f)] public float strength = 1f;

        private TriValidationResult ValidateNormalMap()
        {
#if UNITY_EDITOR
            TextureImporter importer = (TextureImporter) AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(displacementNormal));
            if (importer.textureType != TextureImporterType.NormalMap)
            {
                return TriValidationResult.Error("Normal map is required");
            }
#endif
            return TriValidationResult.Valid;
        }

        public override void SetProperties()
        {
            base.SetProperties();
            propBlock.SetTexture("_Image", noiseTexture);
            propBlock.SetTexture("_Displacement", displacementNormal);
            propBlock.SetFloat("_DispStrength", displacementStrength);
            propBlock.SetFloat("_Strength", strength);
        }
    }
}
