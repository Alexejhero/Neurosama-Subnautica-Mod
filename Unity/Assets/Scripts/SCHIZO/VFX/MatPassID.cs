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
    public MatPassID(Material material) { _mat = material; _id = 0; }

    public MatPassID(Material material, int passID) : this(material) { _id = Mathf.Clamp(passID, 0, mat.passCount - 1);}

    public MatPassID(Material material, BlendMode blendMode) : this(material) { _id = Mathf.Clamp((int)blendMode, 0, mat.passCount - 1); }

    private int _id;
    public int id { get { return _id; } }

    private Material _mat;
    public Material mat { get { return _mat; } }

}
