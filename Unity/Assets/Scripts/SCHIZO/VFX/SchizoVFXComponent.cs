using UnityEngine;

namespace SCHIZO.VFX
{
    public sealed partial class SchizoVFXComponent : MonoBehaviour
    {
        public Material effectMaterial;
        private int posID = Shader.PropertyToID("_ScreenPosition");
        private int distID = Shader.PropertyToID("_Distance");

        private void LateUpdate()
        {
            Vector3 pos = transform.position;
            float distance = Vector3.Distance(Camera.main.transform.position , pos);
            Vector3 screenpos = Camera.main.WorldToScreenPoint(pos);
            effectMaterial.SetVector(posID, new Vector4(screenpos.x, screenpos.y, screenpos.z, Random.Range(-1f, 1f)));
            effectMaterial.SetFloat(distID, distance);
        }
    }
}
