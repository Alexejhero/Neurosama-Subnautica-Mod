using TriInspector;
using UnityEngine;

namespace SCHIZO.VFX
{

    public class SchizoVFXComponent : MonoBehaviour
    {
        [InfoBox("Component for demonstration and test purpose, manual effect implementation is preferable. ")]

        public Effects effect; // select effect to use 

        public Texture2D texture; // if texture is required in Effects description 
        public TextureWrapMode wrapMode; 

        public Texture2D displacement; // normal texture if required in Effects
        public TextureWrapMode displacementWrapMode;

        public Color color; // Colors, if required in Effects
        public Color color2;

        [Range(0f, 1f)]
        public float strength = 1.0f; // opacity and other parameters, if required in Effects

        [InfoBox("Blend mode of main image/color (if material supports it)")]
        public BlendMode blendMode; // blend mode (if effect supports blend modes, the default one is "Add")

        private MatPassID mat; //  material properties holder that stores effect to use and blend mode 

        private void Awake()
        {
            _ = SchizoVFXStack.VFXStack; // check for vfx stack 

            mat = new MatPassID(effect, blendMode); // new instance of MatPassID, with effect to use and blend mode (can be omitted if none required)

            if (texture) texture.wrapMode = wrapMode; 
            if (displacement) displacement.wrapMode = displacementWrapMode;
        }

        public void Update()
        {
            // update properties Effects effect that need to be updated, if property does not need to be updated, it can be initialized in Awake
            mat.SetFloat("_Strength", strength);
            mat.SetTexture("_Image", texture);
            mat.SetTexture("_Displacement", displacement);
            mat.SetColor("_Color", color);
            mat.SetColor("_Color0", color2);

            SchizoVFXStack.RenderEffect(mat); // add MatPassID to the render stack for current frame.
        }
    }
}
