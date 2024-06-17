using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SCHIZO.SwarmControl
{
    [AddComponentMenu("SCHIZO/Swarm Control/Captcha")]
    public sealed partial class Captcha
#if UNITY_EDITOR
        : MonoBehaviour
#endif
    {
        [Tooltip("After this many seconds, if the player hasn't solved the captcha, they die"), Min(0)]
        public float timeout;
        public Image panelBackground;
        public Image image;
        public TMP_Text timerText;
        public TMP_InputField input;
        public CaptchaData[] data;

        [Serializable]
        public partial class CaptchaData
        {
            public Sprite image;
            [FormerlySerializedAs("text")]
            public string textRegex;
        }

        // /ScreenCanvas/Confirmation
        //  uGUI_SceneConfirmation
        //  uGUI_Block
        //  /Panel
        //   Image (background red)
        //   uGUI_NavigableControlGrid
        //   /Description
        //    TextMeshProUGUI
        //   /ButtonOK
        //    Image
        //    Button
        //    TranslationLiveUpdate
        //    /Text
        //     TextMeshProUGUI
    }
}
