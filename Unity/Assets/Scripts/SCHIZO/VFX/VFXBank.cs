using UnityEngine;
namespace SCHIZO.VFX
{
    public static class VFXBank
    {
        public static Material NoisyVignette { get => new Material(Shader.Find("ScreenNoiseVignette")); private set { return; } }
    }
}
