using UnityEngine;
using System.Collections.Generic;

namespace SCHIZO.VFX
{
    public partial class SchizoVFXStack : MonoBehaviour
    {
        public static List<Material> effectMaterials = new List<Material>();

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
            foreach (Material effectMaterial in effectMaterials)
            {
                Graphics.Blit(startedWithA ? tempA : tempB, startedWithA ? tempB : tempA, effectMaterial);
                startedWithA = !startedWithA;
            }

            Graphics.Blit(startedWithA ? tempA : tempB, destination);
            RenderTexture.ReleaseTemporary(tempA);
            RenderTexture.ReleaseTemporary(tempB);
            effectMaterials.Clear();
        }
    }
}
