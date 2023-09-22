using System;
using System.Collections.Generic;
using Nautilus.Utility;
using Nautilus.Utility.MaterialModifiers;
using SCHIZO.Unity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SCHIZO.Helpers;

public static class MaterialHelpers
{
    public static void ApplySNShadersIncludingRemaps(
        GameObject gameObject,
        float shininess = 4f,
        float specularIntensity = 1f,
        float glowStrength = 1f,
        params MaterialModifier[] modifiers)
    {
        Transform disabledParent = new GameObject
        {
            transform =
            {
                parent = gameObject.transform
            }
        }.transform;
        disabledParent.gameObject.SetActive(false);

        Dictionary<MaterialRemapOverride, MeshRenderer> renderers = new();

        foreach (MaterialRemapper remapper in gameObject.GetComponentsInChildren<MaterialRemapper>(true))
        {
            foreach (MaterialRemapOverride remapOverride in remapper.config!?.remappings ?? Array.Empty<MaterialRemapOverride>())
            {
                if (renderers.ContainsKey(remapOverride)) continue;

                GameObject holder = new()
                {
                    transform =
                    {
                        parent = disabledParent
                    }
                };
                MeshRenderer rend = holder.AddComponent<MeshRenderer>();
                rend.materials = remapOverride.materials;
                renderers[remapOverride] = rend;
            }
        }

        MaterialUtils.ApplySNShaders(gameObject, shininess, specularIntensity, glowStrength, modifiers);

        foreach (KeyValuePair<MaterialRemapOverride, MeshRenderer> pair in renderers)
        {
            pair.Key.materials = pair.Value.materials;
        }

        Object.Destroy(disabledParent.gameObject);
    }
}
