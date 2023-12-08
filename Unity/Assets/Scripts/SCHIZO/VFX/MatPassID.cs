using System.Collections.Generic;
using UnityEngine;

namespace SCHIZO.VFX;

/// <summary>
/// Material Properties can be set via <c>SetVector()</c>, <c>SetFloat()</c>, <c>SetColor()</c>, <c>SetTexture()</c> methods.
/// </summary>
public sealed class MatPassID(Material material)
{
    public MatPassID(Material material, int passID) : this(material) { _passID = passID; }

    private int _passID;
    private Dictionary <int, Vector4> vectors;
    public void SetVector(string name, Vector4 value)
    {
        int id = Shader.PropertyToID(name);

        vectors ??= [];
        vectors[id] = value;
    }

    private Dictionary<int, float> floats;
    public void SetFloat(string name, float value)
    {
        int id = Shader.PropertyToID(name);

        floats ??= [];
        floats[id] = value;
    }

    private Dictionary<int, Color> colors;
    public void SetColor(string name, Color value)
    {
        int id = Shader.PropertyToID(name);

        colors ??= [];
        colors[id] = value;
    }

    private Dictionary<int, Texture> textures;
    public void SetTexture(string name, Texture value)
    {
        int id = Shader.PropertyToID(name);

        textures ??= [];
        textures[id] = value;
    }

    public Material ApplyProperties(out int passID)
    {
        passID = Mathf.Clamp(_passID, 0, material.passCount - 1);

        if(vectors != null && vectors.Count != 0)
        {
            foreach (KeyValuePair<int, Vector4> v in vectors)
            {
                material.SetVector(v.Key, v.Value);
            }
        }

        if(floats != null && floats.Count != 0)
        {
            foreach (KeyValuePair<int, float> f in floats)
            {
                material.SetFloat(f.Key, f.Value);
            }
        }

        if(colors != null && colors.Count != 0)
        {
            foreach (KeyValuePair<int, Color> c in colors)
            {
                material.SetColor(c.Key, c.Value);
            }
        }

        if(textures != null && textures.Count != 0)
        {
            foreach (KeyValuePair<int, Texture> t in textures)
            {
                material.SetTexture(t.Key, t.Value);
            }
        }
        return material;
    }
}
