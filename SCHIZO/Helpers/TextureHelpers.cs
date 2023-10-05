namespace SCHIZO.Helpers;

public static class TextureHelpers
{
    public static Sprite CreateSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    public static Texture2D BlendAlpha(Texture2D baseTex, Texture2D appliedTex, float blend = 1f, bool clipToBase = false)
    {
        // Scale texture b to the size of texture a
        Texture2D scaledB = appliedTex.Scale(baseTex.width, baseTex.height);
        Color[] scaledBPixels = scaledB.GetPixels();
        // Blend textures based on transparency
        Texture2D result = new(baseTex.width, baseTex.height, baseTex.format, false);
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

        for (int i = 0; i < original.Length; i++)
            rotated[original.Length-1-i] = original[i];

        Texture2D rotatedTexture = new Texture2D(originalTexture.height, originalTexture.width);
        rotatedTexture.SetPixels(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }

    public static Texture2D Scale(this Texture2D texture, int width, int height)
    {
        Color[] scaledPixels = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color bColor = texture.GetPixelBilinear((float) x / width, (float) y / height);
                scaledPixels[y * width + x] = bColor;
            }
        }

        Texture2D scaled = new(width, height, texture.format, false);
        scaled.SetPixels(scaledPixels);
        scaled.Apply();
        return scaled;
    }

    /// <summary>"Moves" each pixel in the texture by the specified X and Y offsets. Pixels that are moved outside the bounds of the texture are discarded.</summary>
    /// <param name="texture"></param>
    /// <param name="transX">
    /// How much to move on the X axis.<br/>
    /// Positive values will discard pixels from the right edge, and negative values will discard from the left edge.
    /// </param>
    /// <param name="transY">
    /// How much to move on the Y axis.<br/>
    /// Positive values will discard pixels from the top edge, and negative values will discard from the bottom edge.
    /// </param>
    /// <returns>A <see cref="Texture2D"/> with the same size as the original <paramref name="texture"/>.</returns>
    public static Texture2D Translate(this Texture2D texture, int transX, int transY)
    {
        int w = texture.width, h = texture.height;
        Texture2D translated = new(w, h, texture.format, false);
        Color[] originalPixels = texture.GetPixels();
        Color[] translatedPixels = new Color[originalPixels.Length];

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

        translated.SetPixels(translatedPixels);
        translated.Apply();
        return translated;
    }

    // width and height should be less than texture's width/height respectively
    public static Texture2D Crop(this Texture2D texture, int width, int height)
    {
        if (width > texture.width || height > texture.height)
            throw new ArgumentOutOfRangeException();
        Texture2D cropped = new(width, height, texture.format, false);
        Color[] originalPixels = texture.GetPixels();
        Color[] croppedPixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                croppedPixels[y * width + x] = originalPixels[y * texture.width + x];
            }
        }

        cropped.SetPixels(croppedPixels);
        cropped.Apply();
        return cropped;
    }
}
