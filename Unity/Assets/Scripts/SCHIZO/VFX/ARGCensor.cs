using System.Collections;
using TriInspector;
using UnityEngine;

namespace SCHIZO.VFX
{
    public class ARGCensor : MonoBehaviour
    {
        private static readonly int _texID = Shader.PropertyToID("_Images");

        [InlineEditor]
        public Material effectMaterial;

        public float fadeOutStartDistance = 35f;
        public float scale = 1.3f;
        public float frameChangeInterval = 0.08f;

        private CustomMaterialPropertyBlock propBlock;

        private float lastUpdate;
        private float lastRnd;
        private float arrayDepth;

        public void Awake()
        {
            propBlock = new CustomMaterialPropertyBlock(effectMaterial);

            lastUpdate = Time.time;
            arrayDepth = ((Texture2DArray) effectMaterial.GetTexture(_texID)).depth;
        }

        public IEnumerator Start()
        {
            yield return new WaitUntil(() => Camera.main);
            _ = SchizoVFXStack.Instance;
        }

        public void LateUpdate()
        {
            if (!Camera.main) return;
            Vector3 dirToCam = (Camera.main.transform.position - transform.position).normalized;
            Vector3 pos = Camera.main.WorldToScreenPoint(transform.position + dirToCam);

            float dot = Vector3.Dot(transform.forward, dirToCam);

            bool isBelowWaterLevel = Camera.main.transform.position.y < 0f;

            if (isBelowWaterLevel && pos.z > 0 && dot > -0.3f)
            {
                if (Time.time - lastUpdate > frameChangeInterval)
                {
                    lastRnd = Random.Range(0, arrayDepth) * Time.timeScale;
                    lastUpdate = Time.time;
                }

                float opacity = Mathf.Clamp01((1f / pos.z) * fadeOutStartDistance);

                propBlock.SetVector("_ScreenPosition", new Vector4(pos.x, pos.y, pos.z, lastRnd));
                propBlock.SetFloat("_Strength", opacity);
                propBlock.SetFloat("_Scale", scale);

                SchizoVFXStack.Instance.RenderEffect(propBlock);
            }
        }
    }
}
