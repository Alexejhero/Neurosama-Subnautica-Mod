using SCHIZO.Options.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SCHIZO.Options.Button
{
    [CreateAssetMenu(menuName = "SCHIZO/Options/Button Mod Option")]
    public sealed partial class ButtonModOption : ModOption
    {
        public UnityEvent onPressed;
    }
}
