using UnityEngine;

namespace SCHIZO
{
    public static class TextureUtils
    {
        public static Texture2D CombineTextures(Texture2D hullplateIcon, Texture2D hullplateTexture)
        {
            // Scale texture b to the size of texture a
            Texture2D scaledB = new Texture2D(hullplateIcon.width, hullplateIcon.height);
            Color[] scaledBPixels = scaledB.GetPixels(0, 0, hullplateIcon.width, hullplateIcon.height);
            for (int y = 0; y < hullplateIcon.height; y++)
            {
                for (int x = 0; x < hullplateIcon.width; x++)
                {
                    Color bColor = hullplateTexture.GetPixelBilinear((float) x / hullplateIcon.width, (float) y / hullplateIcon.height);
                    scaledBPixels[y * hullplateIcon.width + x] = bColor;
                }
            }

            scaledB.SetPixels(scaledBPixels);
            scaledB.Apply();
            // Blend textures based on transparency
            Texture2D result = new Texture2D(hullplateIcon.width, hullplateIcon.height);
            Color[] resultPixels = result.GetPixels(0, 0, hullplateIcon.width, hullplateIcon.height);
            for (int i = 0; i < resultPixels.Length; i++)
            {
                Color aColor = hullplateIcon.GetPixel(i % hullplateIcon.width, i / hullplateIcon.width);
                Color bColor = scaledB.GetPixel(i % hullplateIcon.width, i / hullplateIcon.width);
                float blendFactor = bColor.a;
                Color blendedColor = Color.Lerp(aColor, bColor, blendFactor);
                resultPixels[i] = blendedColor;
            }

            result.SetPixels(resultPixels);
            result.Apply();

            return result;
        }
    }
}
