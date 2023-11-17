using UnityEngine;
using System.Collections.Generic;

namespace SCHIZO.VFX
{
    public class SchizoVFXStack : MonoBehaviour
    {
        private static SchizoVFXStack _instance;

        public static SchizoVFXStack VFXStack
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = Camera.main.GetComponent<SchizoVFXStack>();
                if (_instance == null) _instance = Camera.main.gameObject.AddComponent<SchizoVFXStack>();
                return _instance;
            }
        }

        private static List<MatPassID> effectMaterials = new List<MatPassID>();

        public static void RenderEffect(MatPassID m)
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
                Graphics.Blit(startedWithA ? tempA : tempB, startedWithA ? tempB : tempA, effectMaterial.mat, effectMaterial.id);
                startedWithA = !startedWithA;
            }

            Graphics.Blit(startedWithA ? tempA : tempB, destination);
            RenderTexture.ReleaseTemporary(tempA);
            RenderTexture.ReleaseTemporary(tempB);
            effectMaterials.Clear();
        }
    }
}
