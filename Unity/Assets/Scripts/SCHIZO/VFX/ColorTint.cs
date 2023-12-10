using SCHIZO.VFX;
using UnityEngine;

public class ColorTint : VFXComponent
{
    public Color color;
    public BlendMode blendMode;

    public override void SetProperties()
    {
        base.SetProperties();
        matPassID.SetColor("_Color", color);
        matPassID.passID = (int) blendMode;
    }

    public override void Update()
    {
        base.Update();
    }
}
