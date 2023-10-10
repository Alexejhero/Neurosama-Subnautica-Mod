using SCHIZO.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using UnityEngine;

namespace SCHIZO.Tweaks;

[LoadComponent]
public sealed class ErmMoon : MonoBehaviour
{
    private uSkyManager _skyManager;
    private Texture2D _normalMoonTex;
    private Texture2D _readableMoonTex;
    private Texture2D _ermTex;
    private Texture2D _ermMoonTex;
    private float _normalMoonSize;

    // controls the moon size cycle
    private const float moonSizeTimeScale = 0.4f;
    private const float maxMoonSizeMulti = 3;

    private void FixedUpdate()
    {
        if (_skyManager) return;
        _skyManager = FindObjectOfType<uSkyManager>();
        if (!_skyManager || !_skyManager.MoonTexture) return;

        _normalMoonSize = _skyManager.MoonSize;

        _ermTex = Assets.Old_Erm_Icons_Erm.texture.GetReadable();
        _ermTex = _ermTex.Rotate180(); // moon texture is upside down
        _ermTex.name = "erm";

        _normalMoonTex = _skyManager.MoonTexture;
        _readableMoonTex = _normalMoonTex.GetReadable();

        _ermMoonTex = TextureHelpers.BlendAlpha(_readableMoonTex, _ermTex, 0.30f, true);
        _ermMoonTex.wrapMode = _normalMoonTex.wrapMode;
        _ermMoonTex.Apply(false, true); // send to gpu
        _ermMoonTex.name = _normalMoonTex.name + "_erm";
    }

    private void Update()
    {
        if (!_skyManager) return;
        ToggleErmDeity(CONFIG.EnableErmMoon);
        float ermMoonSize = CONFIG.EnableErmMoon
            ? _normalMoonSize * (1 + Mathf.PingPong(0.5f + GetCurrentDay() * moonSizeTimeScale, maxMoonSizeMulti - 1))
            : _normalMoonSize;
        UpdateErmMoon(ermMoonSize);
    }

    private void ToggleErmDeity(bool isVisible)
    {
        _skyManager.SkyboxMaterial.SetTexture(ShaderPropertyID._MoonSampler, isVisible ? _ermMoonTex : _normalMoonTex);
    }

    private void UpdateErmMoon(float size)
    {
        _skyManager.SkyboxMaterial.SetFloat(ShaderPropertyID._MoonSize, size);
    }

    private static float GetCurrentDay() => (float)(DayNightCycle.main!?.GetDay() ?? 0f);
}
