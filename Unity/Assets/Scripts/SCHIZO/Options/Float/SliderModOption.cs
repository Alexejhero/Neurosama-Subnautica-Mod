using SCHIZO.Options.Generic;
using UnityEngine;

namespace SCHIZO.Options.Float
{
    [CreateAssetMenu(menuName = "SCHIZO/Options/Slider Mod Option")]
    public sealed partial class SliderModOption : ModOption<float>
    {
        public float min;
        public float max;
        public float step = 1f;
        public string valueFormat;
    }
}
