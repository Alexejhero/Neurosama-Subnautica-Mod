using TriInspector;
using UnityEngine;

namespace SCHIZO.VFX
{
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
            matPassID.PassID = (int) blendMode;
            image.wrapMode = wrapMode;
            matPassID.SetTexture("_Image", image);
            matPassID.SetVector("_Position", new Vector4(position.x, position.y, 0f, 0f));
            matPassID.SetFloat("_Strength", strength);
            matPassID.SetFloat("_Scale", scale);
        }
    }
}
