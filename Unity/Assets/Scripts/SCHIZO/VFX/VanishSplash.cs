using System.Collections;
using UnityEngine;

namespace SCHIZO.VFX
{
    [AddComponentMenu("SCHIZO/VFX/Vanish")]
    public class VanishSplash : VFXComponent
    {
        public Texture2D image;
        public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
        public AnimationCurve curve;
        public float displacementStrength = 1f;
        public float effectDuration = 1f;

        [Range(0f, 10f)]
        public float scale = 1;

        public float fadeoutDist = 35f;
        private float opacity;

        private Vector3 pos;
        private float phase = 0;

        public override void SetProperties()
        {
            base.SetProperties();
            image.wrapMode = wrapMode;
            propBlock.SetTexture("_Image", image);
            propBlock.SetVector("_ScreenPosition", new Vector4(pos.x, pos.y, pos.z, 0f));
            propBlock.SetFloat("_Strength", opacity);
            propBlock.SetFloat("_Scale", scale);
            propBlock.SetFloat("_Phase", phase);
            propBlock.SetFloat("_DispStr", displacementStrength);
        }

        public void Play()
        {
            StartCoroutine(Playing());
        }

        private IEnumerator Playing()
        {
            Vector3 dirToCam = (Camera.main.transform.position - transform.position).normalized;
            pos = Camera.main.WorldToScreenPoint(transform.position + dirToCam);
            opacity = Mathf.Clamp01((1f / pos.z) * fadeoutDist);

            for (float t = 0; t < effectDuration; t += Time.deltaTime)
            {
                phase = curve.Evaluate(t);
                yield return null;
            }
        }
    }
}
