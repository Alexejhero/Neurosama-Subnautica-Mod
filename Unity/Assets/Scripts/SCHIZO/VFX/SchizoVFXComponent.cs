using UnityEngine;

namespace SCHIZO.VFX
{
    public partial class SchizoVFXComponent : MonoBehaviour
    {
        public MatWithProps matWithProps;
        public void SendEffect(MatWithProps matWithProps)
        {
            SchizoVFXStack VFXstack = Camera.main.gameObject.GetComponent<SchizoVFXStack>();
            if ( VFXstack == null ) Camera.main.gameObject.AddComponent<SchizoVFXStack>();
            VFXstack.effectMaterials.Add(matWithProps);
        }
    }
}
