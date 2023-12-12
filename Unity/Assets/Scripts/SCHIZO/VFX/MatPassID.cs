using System.Collections.Generic;
using UnityEngine;

namespace SCHIZO.VFX
{
    /// <summary>
    /// Material Properties can be set via <see cref="SetVector"/>, <see cref="SetFloat"/>, <see cref="SetColor"/>, <see cref="SetTexture"/> methods.
    /// </summary>
    public sealed class MatPassID(Material material)
    {
        public int PassID { get; set; }

        private Dictionary<int, Vector4> _vectors;
        private Dictionary<int, float> _floats;
        private Dictionary<int, Color> _colors;
        private Dictionary<int, Texture> _textures;

        public void SetVector(string name, Vector4 value)
        {
            int id = Shader.PropertyToID(name);

            _vectors ??= [];
            _vectors[id] = value;
        }


        public void SetFloat(string name, float value)
        {
            int id = Shader.PropertyToID(name);

            _floats ??= [];
            _floats[id] = value;
        }

        public void SetColor(string name, Color value)
        {
            int id = Shader.PropertyToID(name);

            _colors ??= [];
            _colors[id] = value;
        }

        public void SetTexture(string name, Texture value)
        {
            int id = Shader.PropertyToID(name);

            _textures ??= [];
            _textures[id] = value;
        }

        public Material ApplyProperties(out int passID)
        {
            passID = Mathf.Clamp(PassID, 0, material.passCount - 1);

            if (_vectors != null && _vectors.Count != 0)
            {
                foreach (KeyValuePair<int, Vector4> v in _vectors)
                {
                    material.SetVector(v.Key, v.Value);
                }
            }

            if (_floats != null && _floats.Count != 0)
            {
                foreach (KeyValuePair<int, float> f in _floats)
                {
                    material.SetFloat(f.Key, f.Value);
                }
            }

            if (_colors != null && _colors.Count != 0)
            {
                foreach (KeyValuePair<int, Color> c in _colors)
                {
                    material.SetColor(c.Key, c.Value);
                }
            }

            if (_textures != null && _textures.Count != 0)
            {
                foreach (KeyValuePair<int, Texture> t in _textures)
                {
                    material.SetTexture(t.Key, t.Value);
                }
            }

            return material;
        }
    }
}
