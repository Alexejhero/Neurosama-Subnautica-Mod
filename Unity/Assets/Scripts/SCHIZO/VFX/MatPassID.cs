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
    public MatPassID(Effects effect) { _effect = effect; _passID = 0; }
    public MatPassID(Effects effect, int passID) : this(effect) { _passID = passID; }
    public MatPassID(Effects effect, BlendMode blendMode) : this(effect) { _passID = (int) blendMode; }

    private int _passID;
    private Effects _effect;

    private Dictionary <int, Vector4> vectors;
    public void SetVector(string name, Vector4 value)
    {
        int id = Shader.PropertyToID(name);

        vectors ??= [];
        if (vectors.ContainsKey(id)) { vectors[id] = value; }
        else { vectors.Add(id, value); }
    }

    private Dictionary<int, float> floats;
    public void SetFloat(string name, float value)
    {
        int id = Shader.PropertyToID(name);

        floats ??= [];
        if (floats.ContainsKey(id)) { floats[id] = value; }
        else{ floats.Add(id, value); }
    }

    private Dictionary<int, Color> colors;
    public void SetColor(string name, Color value)
    {
        int id = Shader.PropertyToID(name);

        colors ??= [];
        if (colors.ContainsKey(id)) { colors[id] = value; }
        else { colors.Add(id, value); }
    }

    private Dictionary<int, Texture> textures;
    public void SetTexture(string name, Texture value)
    {
        int id = Shader.PropertyToID(name);

        textures ??= [];
        if (textures.ContainsKey(id)) { textures[id] = value; }
        else { textures.Add(id, value); }
    }

    public Material ApplyProperties(out int ID)
    {
        Material _mat = VFXMaterialHolder.instance.GetMaterialForEffect(_effect);
        ID = Mathf.Clamp(_passID, 0, _mat.passCount - 1);

        if(vectors != null && vectors.Count != 0)
        {
            foreach (KeyValuePair<int, Vector4> v in vectors)
            {
                _mat.SetVector(v.Key, v.Value);
            }
        }

        if(floats != null && floats.Count != 0)
        {
            foreach (KeyValuePair<int, float> f in floats)
            {
                _mat.SetFloat(f.Key, f.Value);
            }
        }

        if(colors != null && colors.Count != 0)
        {
            foreach (KeyValuePair<int, Color> c in colors)
            {
                _mat.SetColor(c.Key, c.Value);
            }
        }

        if(textures != null && textures.Count != 0)
        {
            foreach (KeyValuePair<int, Texture> t in textures)
            {
                _mat.SetTexture(t.Key, t.Value);
            }
        }
        return _mat;
    }
}
