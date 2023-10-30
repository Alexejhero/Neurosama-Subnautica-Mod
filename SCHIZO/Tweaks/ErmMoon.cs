using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Tweaks;

partial class ErmMoon
{
    private void Awake()
    {
        uSkyManager _skyManager = GetComponentInParent<uSkyManager>();

        Texture2D rotatedErmTexture = ermTexture.Rotate180(); // moon texture is upside down
        Texture2D normalTexture = _skyManager.MoonTexture;

        Texture2D ermMoonTexture = TextureHelpers.BlendAlpha(normalTexture.GetReadable(), rotatedErmTexture, 0.30f, true);
        ermMoonTexture.wrapMode = normalTexture.wrapMode;
        ermMoonTexture.Apply(false, true); // send to gpu

        _skyManager.MoonTexture = ermMoonTexture;
        _skyManager.MoonSize *= 2;
    }
}
