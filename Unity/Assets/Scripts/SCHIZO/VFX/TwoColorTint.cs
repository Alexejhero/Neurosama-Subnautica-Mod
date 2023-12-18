using UnityEngine;

namespace SCHIZO.VFX
{
    [AddComponentMenu("SCHIZO/VFX/Two Color Tint")]
    public class TwoColorTint : VFXComponent
    {
        public Color innerColor = Color.red;
        public Color outerColor = Color.green;
        [Range(0f, 1f)]
        public float strength = 0.3f;

        public override void SetProperties()
        {
            base.SetProperties();
            propBlock.SetColor("_Color", innerColor);
            propBlock.SetColor("_Color0", outerColor);
            propBlock.SetFloat("_Strength", strength);
        }
    }
}
