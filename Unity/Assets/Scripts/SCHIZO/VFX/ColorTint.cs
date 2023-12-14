using UnityEngine;

namespace SCHIZO.VFX
{
    [AddComponentMenu("SCHIZO/VFX/Color Tint")]
    public class ColorTint : VFXComponent
    {
        public Color color;
        public BlendMode blendMode;

        public override void SetProperties()
        {
            base.SetProperties();
            propBlock.SetColor("_Color", color);
            propBlock.PassID = (int) blendMode;
        }
    }
}
