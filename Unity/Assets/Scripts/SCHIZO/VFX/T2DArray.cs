using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[CreateAssetMenu(menuName ="SCHIZO/VFX/Texture2DArray")]
public class T2DArray : ScriptableObject
{
    [InfoBox("All Textures must have same dimensions!", TriMessageType.Error, nameof(texturesDimensionsMismatch))]
    [InfoBox("Please assign at least 2 textures, or use Texture2D if single texture is intended", TriMessageType.Warning, nameof(tooFewTextures))]
    [Required]
    public List<Texture> textures;

    private bool texturesDimensionsMismatch = true;
    private bool tooFewTextures = true;

    private void OnValidate()
    {
        if (textures.Count > 1 ) ClearEmpty(textures);
        tooFewTextures = textures.Count < 2;

        if (textures.Count > 1 && textures[0] != null)
        {
            texturesDimensionsMismatch = false;
            int w = textures[0].width;
            int h = textures[0].height;

            foreach (Texture texture in textures)
            {
                if (texture.width != w || texture.height != h)
                {
                    texturesDimensionsMismatch = true;
                    return;
                }
            }
        }
    }

    private List<T> ClearEmpty <T>(List<T> list)
    {
        list.RemoveAll(o => o.GetType() == typeof(T));
        return list;
    }

    public Texture2DArray PopulateTexture2DArray()
    {
        List<Texture> tt = textures;
        Texture2DArray t2da = new Texture2DArray(tt[0].width, tt[0].height, tt.Count, TextureFormat.RGBAHalf, false);

        for (int i = 0; i < tt.Count; i++)
        {
            Graphics.ConvertTexture(tt[i], 0, t2da, i);
        }
        return t2da;
    }
}
