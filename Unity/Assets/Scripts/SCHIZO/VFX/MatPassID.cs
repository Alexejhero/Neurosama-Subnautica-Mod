using System.Collections.Generic;
using UnityEngine;

namespace SCHIZO.VFX;
public enum BlendMode
{
    Add = 0,
    Normal = 1,
    Multiply = 2,
    Screen = 3,
    Substract = 4,
}

public sealed class MatPassID
{
    public MatPassID(Material material) { _mat = material; _passID = 0; }
    public MatPassID(Material material, int passID) : this(material) { _passID = Mathf.Clamp(passID, 0, mat.passCount - 1);}
    public MatPassID(Material material, BlendMode blendMode) : this(material) { _passID = Mathf.Clamp((int)blendMode, 0, mat.passCount - 1); }

    private int _passID;
    public int passID { get => _passID; }

    private Material _mat;
    public Material mat { get => _mat; }

    private Dictionary <int, Vector4> vectors = [];
    public void SetVector(string name, Vector4 value)
    {
        int id = Shader.PropertyToID(name);

        if (vectors.ContainsKey(id)) { vectors[id] = value; }
        else { vectors.Add(id, value); }
    }

    private Dictionary<int, float> floats = [];
    public void SetFloat(string name, float value)
    {
        int id = Shader.PropertyToID(name);

        if (floats.ContainsKey(id)) { floats[id] = value; }
        else{ floats.Add(id, value); }
    }

    private Dictionary<int, Color> colors = [];
    public void SetColor(string name, Color value)
    {
        int id = Shader.PropertyToID(name);

        if (colors.ContainsKey(id)) { colors[id] = value; }
        else { colors.Add(id, value); }
    }

    private Dictionary<int, Texture> textures = [];
    public void SetTexture(string name, Texture value)
    {
        int id = Shader.PropertyToID(name);

        if (textures.ContainsKey(id)) { textures[id] = value; }
        else { textures.Add(id, value); }
    }

    public Material ApplyProperties()
    {
        if(vectors.Count != 0)
        {
            foreach (KeyValuePair<int, Vector4> v in vectors)
            {
                _mat.SetVector(v.Key, v.Value);
            }
        }

        if(floats.Count != 0)
        {
            foreach(KeyValuePair<int, float> f in floats)
            {
                _mat.SetFloat(f.Key, f.Value);
            }
        }

        if(colors.Count != 0)
        {
            foreach(KeyValuePair<int, Color> c in colors)
            {
                _mat.SetColor(c.Key, c.Value);
            }
        }

        if(textures.Count != 0)
        {
            foreach(KeyValuePair<int, Texture> t in textures)
            {
                _mat.SetTexture(t.Key, t.Value);
            }
        }
        return _mat;
    }
}
