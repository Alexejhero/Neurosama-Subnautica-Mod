using UnityEngine;
using System.Collections.Generic;

namespace SCHIZO.VFX;

public class SchizoVFXStack : MonoBehaviour
{
    private static SchizoVFXStack _instance;

    public static SchizoVFXStack Instance
    {
        get
        {
            if (_instance) return _instance;
            return _instance = Camera.main!.gameObject.AddComponent<SchizoVFXStack>();
        }
    }

    private static readonly List<MatPassID> effectMaterials = [];

    public void RenderEffect(MatPassID m)
    {
        if (effectMaterials.Contains(m)) return;
        effectMaterials.Add(m);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (effectMaterials.Count == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }

        RenderTexture tempA = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        RenderTexture tempB = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        Graphics.CopyTexture(source, tempA);

        bool startedWithA = true;
        foreach (MatPassID effectMaterial in effectMaterials)
        {
            Graphics.Blit(startedWithA ? tempA : tempB, startedWithA ? tempB : tempA, effectMaterial.ApplyProperties(out int passID), passID);
            startedWithA = !startedWithA;
        }

        Graphics.Blit(startedWithA ? tempA : tempB, destination);
        RenderTexture.ReleaseTemporary(tempA);
        RenderTexture.ReleaseTemporary(tempB);
        effectMaterials.Clear();
    }
}
