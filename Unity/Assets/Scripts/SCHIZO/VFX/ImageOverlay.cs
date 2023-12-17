using TriInspector;
using UnityEngine;

namespace SCHIZO.VFX
{
    [AddComponentMenu("SCHIZO/VFX/Image Overlay")]
    public class ImageOverlay : VFXComponent
    {
        [InfoBox("Despite appearing stretched on non 1:1 aspect ratio previews, it should display correct in game.")]
        [InfoBox("To avoid artifacts, image must have a border of transparent pixels on the edges.")]
        public Texture2D image;
        public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
        public BlendMode blendMode = BlendMode.Normal;

        [Range(0f, 10f)]
        public float scale = 1;
        [Range(0f, 1f)]
        public float strength = 0.1f;

        public Vector2 position;
        public override void SetProperties()
        {
            base.SetProperties();
            propBlock.PassID = (int) blendMode;
            image.wrapMode = wrapMode;
            propBlock.SetTexture("_Image", image);
            propBlock.SetVector("_Position", new Vector4(position.x, position.y, 0f, 0f));
            propBlock.SetFloat("_Strength", strength);
            propBlock.SetFloat("_Scale", scale);
        }
    }
}
