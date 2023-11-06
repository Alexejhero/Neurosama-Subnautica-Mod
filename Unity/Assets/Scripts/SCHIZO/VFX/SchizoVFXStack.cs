using UnityEngine;
using System.Collections.Generic;

namespace SCHIZO.VFX
{
    public partial class SchizoVFXStack : MonoBehaviour
    {
        public List<MatWithProps> effectMaterials = new List<MatWithProps>();

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
            foreach (MatWithProps effectMaterial in effectMaterials)
            {
                Material m = effectMaterial.material;
                m.SetFloat(effectMaterial.floatPropertyID, effectMaterial.floatPropertyValue);
                m.SetVector(effectMaterial.vectorPropertyID, effectMaterial.vectorPropertyValue);
                m.SetColor(effectMaterial.colorPropertyID, effectMaterial.colorPropertyValue);
                Graphics.Blit(startedWithA ? tempA : tempB, startedWithA ? tempB : tempA, m);
                startedWithA = !startedWithA;
            }

            Graphics.Blit(startedWithA ? tempA : tempB, destination);
            RenderTexture.ReleaseTemporary(tempA);
            RenderTexture.ReleaseTemporary(tempB);
            effectMaterials.Clear();
        }
    }
}
