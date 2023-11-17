using SCHIZO.VFX;
using UnityEngine;

public class ColorTint : MonoBehaviour
{
    public Shader shader;
    public Color color;

    public BlendMode blendMode;

    private MatPassID mat;

    private void Awake()
    {
        Material material = new Material(shader);
        mat = new MatPassID(material, blendMode);
        _ = SchizoVFXStack.VFXStack;
        mat.mat.color = color;
    }

    private void Update()
    {
        SchizoVFXStack.RenderEffect(mat);
    }
}
