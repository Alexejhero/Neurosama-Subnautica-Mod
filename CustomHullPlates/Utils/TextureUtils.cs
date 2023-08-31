using UnityEngine;

namespace SCHIZO.Utils;

public static class TextureUtils
{
    public static Texture2D BlendAlpha(Texture2D baseTex, Texture2D appliedTex, float blend = 1f, bool clipToBase = false)
    {
        // Scale texture b to the size of texture a
        Texture2D scaledB = new Texture2D(baseTex.width, baseTex.height, baseTex.format, false);
        Color[] scaledBPixels = scaledB.GetPixels();
        for (int y = 0; y < baseTex.height; y++)
        {
            for (int x = 0; x < baseTex.width; x++)
            {
                Color bColor = appliedTex.GetPixelBilinear((float)x / baseTex.width, (float)y / baseTex.height);
                scaledBPixels[y * baseTex.width + x] = bColor;
            }
        }

        scaledB.SetPixels(scaledBPixels);
        // Blend textures based on transparency
        Texture2D result = new Texture2D(baseTex.width, baseTex.height, baseTex.format, false);
        Color[] resultPixels = BlendAlpha(baseTex.GetPixels(), scaledBPixels, blend, clipToBase);

        result.SetPixels(resultPixels);
        result.Apply();

        return result;
    }
    private static Color[] BlendAlpha(Color[] a, Color[] b, float blend = 1f, bool clipToBase = false)
    {
        Color[] result = new Color[a.Length];
        for (int i = 0; i < result.Length; i++)
        {
            Color aColor = a[i];
            Color bColor = b[i];
            float blendFactor = bColor.a * blend;
            if (clipToBase)
                blendFactor *= aColor.a;
            Color blendedColor = Color.Lerp(aColor, bColor, blendFactor);
            result[i] = blendedColor;
        }
        return result;
    }

    private static Color[] BlendAdditive(Color[] a, Color[] b, float blend = 1f)
    {
        Color[] result = new Color[a.Length];
        for (int i = 0; i < result.Length; i++)
        {
            Color aColor = a[i];
            Color bColor = b[i];
            float blendFactor = bColor.a * blend;
            Color blendedColor = aColor + (bColor * blendFactor);
            blendedColor.a = aColor.a;
            result[i] = blendedColor;
        }
        return result;
    }

    /// <summary>Copies this texture from GPU to CPU so its pixels can be read.</summary>
    /// <returns>
    /// A readable <see cref="Texture2D"/> with the same pixels as this texture.<br/>
    /// If the texture is already readable, it is returned directly.
    /// </returns>
    public static Texture2D GetReadable(this Texture2D texture)
    {
        if (texture.isReadable)
        {
            Debug.LogWarning($"Texture {texture.name} is already readable");
            return texture;
        }
        Texture2D copy = new Texture2D(texture.width, texture.height);
        //Graphics.CopyTexture(tex, copy); // copies GPU to GPU, we can't read that

        RenderTexture tmp = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
        Graphics.Blit(texture, tmp);

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = tmp;

        copy.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        copy.Apply();

        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(tmp);

        return copy;
    }

    public static Texture2D Rotate180(this Texture2D originalTexture)
    {
        Color[] original = originalTexture.GetPixels();
        Color[] rotated = new Color[original.Length];

        for (var i = 0; i < original.Length; i++)
            rotated[original.Length-1-i] = original[i];

        Texture2D rotatedTexture = new Texture2D(originalTexture.height, originalTexture.width);
        rotatedTexture.SetPixels(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }
}
