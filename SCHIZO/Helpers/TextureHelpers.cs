using System;
using UnityEngine;

namespace SCHIZO.Helpers;

public static class TextureHelpers
{
    public static Sprite CreateSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    /// <summary>
    /// Blend two textures using alpha blending.
    /// </summary>
    /// <param name="baseTex">Texture to blend onto, i.e. the "bottom layer".</param>
    /// <param name="appliedTex">Texture to blend with, i.e. the "top layer".</param>
    /// <param name="blend">Lerp proportion, from 0 (only <paramref name="baseTex"/>) to 1 (only <paramref name="appliedTex"/>).</param>
    /// <param name="clipAlphaToBase">
    /// In terms of the result, <see langword="false"/> will directly blend between the two alpha values,<br/>
    /// and <see langword="true"/> more resembles "applying" <paramref name="appliedTex"/> on top of <paramref name="baseTex"/>.
    /// </param>
    /// <remarks>
    /// If the texture dimensions aren't the same, <paramref name="appliedTex"/> will be bilinearly scaled to <paramref name="baseTex"/>.
    /// </remarks>
    public static Texture2D BlendAlpha(Texture2D baseTex, Texture2D appliedTex, float blend = 1f, bool clipAlphaToBase = false)
    {
        // Scale texture b to the size of texture a
        Texture2D scaledB = appliedTex.Scale(baseTex.width, baseTex.height);
        Color32[] scaledBPixels = scaledB.GetPixels32();
        // Blend textures based on transparency
        Texture2D result = new(baseTex.width, baseTex.height, baseTex.format, false);
        Color32[] resultPixels = BlendAlpha(baseTex.GetPixels32(), scaledBPixels, blend, clipAlphaToBase);

        result.SetPixels32(resultPixels);
        result.Apply();

        return result;
    }
    private static Color32[] BlendAlpha(Color32[] a, Color32[] b, float blend = 1f, bool clipToBase = false)
    {
        Color32[] result = new Color32[a.Length];
        for (int i = 0; i < result.Length; i++)
        {
            Color32 aColor = a[i];
            Color32 bColor = b[i];
            float blendFactor = bColor.a * blend;
            if (clipToBase)
                blendFactor *= aColor.a;
            Color32 blendedColor = Color32.Lerp(aColor, bColor, blendFactor);
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
            LOGGER.LogWarning($"Texture {texture.name} is already readable");
            return texture;
        }
        Texture2D copy = new(texture.width, texture.height);
        //Graphics.CopyTexture(tex, copy); // copies GPU to GPU, we can't read that here in CPU land

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
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];

        for (int i = 0; i < original.Length; i++)
            rotated[original.Length-1-i] = original[i];

        Texture2D rotatedTexture = new(originalTexture.height, originalTexture.width);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }

    public static Texture2D Scale(this Texture2D texture, int width, int height)
    {
        if (texture.width == width && texture.height == height)
            return texture;

        Color32[] scaledPixels = new Color32[width * height];
        float widthScaler = 1f / width;
        float heightScaler = 1f / height;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color32 bColor = texture.GetPixelBilinear(x * widthScaler, y * heightScaler);
                scaledPixels[y * width + x] = bColor;
            }
        }

        Texture2D scaled = new(width, height, texture.format, false);
        scaled.SetPixels32(scaledPixels);
        scaled.Apply();
        return scaled;
    }

    /// <summary>"Moves" each pixel in the texture by the specified X and Y offsets. Pixels that are moved outside the bounds of the texture are discarded.</summary>
    /// <param name="transX">
    /// How much to move on the X axis.<br/>
    /// Positive values will discard pixels from the right edge, and negative values will discard from the left edge.
    /// </param>
    /// <param name="transY">
    /// How much to move on the Y axis.<br/>
    /// Positive values will discard pixels from the top edge, and negative values will discard from the bottom edge.
    /// </param>
    /// <returns>A <see cref="Texture2D"/> with the same size as the original texture.</returns>
    public static Texture2D Translate(this Texture2D texture, int transX, int transY)
    {
        int w = texture.width, h = texture.height;
        Texture2D translated = new(w, h, texture.format, false);
        Color32[] originalPixels = texture.GetPixels32();
        Color32[] translatedPixels = new Color32[originalPixels.Length];

        int xMin = 0, xMax = w;
        int yMin = 0, yMax = h;
        if (transX < 0) xMin -= transX;
        else xMax -= transX;
        if (transY < 0) yMin -= transY;
        else yMax -= transY;

        for (int y = yMin; y < yMax; y++)
        {
            for (int x = xMin; x < xMax; x++)
            {
                int translatedX = x + transX;
                int translatedY = y + transY;
                translatedPixels[translatedY * w + translatedX] = originalPixels[y * xMax + x];
            }
        }

        translated.SetPixels32(translatedPixels);
        translated.Apply();
        return translated;
    }

    // width and height should be less than texture's width/height respectively
    public static Texture2D Crop(this Texture2D texture, int width, int height)
    {
        if (width > texture.width || height > texture.height)
            throw new ArgumentOutOfRangeException();
        Texture2D cropped = new(width, height, texture.format, false);
        Color32[] originalPixels = texture.GetPixels32();
        Color32[] croppedPixels = new Color32[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                croppedPixels[y * width + x] = originalPixels[y * texture.width + x];
            }
        }

        cropped.SetPixels32(croppedPixels);
        cropped.Apply();
        return cropped;
    }
}
