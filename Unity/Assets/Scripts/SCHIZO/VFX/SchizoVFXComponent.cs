using UnityEngine;

namespace SCHIZO.VFX
{
    public partial class SchizoVFXComponent : MonoBehaviour
    {
        public Material effectMaterial;
        public void SendEffect(MatWithProps matWithProps)
        {
            SchizoVFXStack VFXstack = Camera.main.gameObject.GetComponent<SchizoVFXStack>();
            if ( VFXstack == null ) { Camera.main.gameObject.AddComponent<SchizoVFXStack>(); }
            Camera.main.gameObject.GetComponent<SchizoVFXStack>().effectMaterials.Add(matWithProps);
        }
    }
}
