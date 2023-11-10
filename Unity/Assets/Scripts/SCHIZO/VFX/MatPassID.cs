using UnityEngine;

public sealed class MatPassID
{
    private MatPassID() { }

    public MatPassID(Material material) : this() { _mat = material; _id = 0; }

    public MatPassID(Material material, int passID) : this(material) { _id = Mathf.Clamp(passID, 0, mat.passCount - 1);}

    private int _id;
    public int id { get { return _id; } }

    private Material _mat;
    public Material mat { get { return _mat; } }

}
