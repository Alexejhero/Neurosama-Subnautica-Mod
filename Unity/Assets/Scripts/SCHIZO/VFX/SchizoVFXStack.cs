using UnityEngine;
using System.Collections.Generic;

namespace SCHIZO.VFX
{
    public partial class SchizoVFXStack : MonoBehaviour
    {
        public List<MatWithProps> effectMaterials = new List<MatWithProps> ();

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (effectMaterials.Count == 0)
            {
                Debug.LogWarning("No effects to render");
                Graphics.Blit (source, destination);
                return;
            }

            RenderTexture tempA = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            RenderTexture tempB = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            Graphics.CopyTexture(source, tempA);

            bool startedWithA = true;
            foreach (MatWithProps effectMaterial in effectMaterials)
            {
                if ( effectMaterial == null)
                {
                    throw new System.Exception("Effect object is missing");
                }
                Material m = effectMaterial.material;
                m.SetVector(effectMaterial.vectorPropertyID, effectMaterial.vectorPropertyValue);
                m.SetColor(effectMaterial.colorPropertyID, effectMaterial.colorPropertyValue);
                Graphics.Blit(startedWithA ? tempA : tempB, startedWithA ? tempB : tempA, m);
                startedWithA = !startedWithA;
            }

            Graphics.Blit(startedWithA ? tempB : tempA, destination);
            RenderTexture.ReleaseTemporary(tempA);
            RenderTexture.ReleaseTemporary(tempB);
            effectMaterials.Clear();
        }
    }
}
