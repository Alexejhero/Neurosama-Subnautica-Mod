using UnityEngine;

namespace SCHIZO.Options
{
    [CreateAssetMenu(menuName = "SCHIZO/Options/Slider Mod Option")]
    public sealed partial class SliderModOption : ModOption<float>
    {
        public ConfigurableValueFloat min;
        public ConfigurableValueFloat max;
        public float step = 1f;
        public string valueFormat;
    }
}
