using UnityEngine;
using UWE;

namespace SCHIZO.VFX
{
    public partial class SchizoVFXComponent
    {
        public void Awake()
        {
            VFXEffect effect = Camera.main.gameObject.AddComponent<VFXEffect>();
            effect.effectMaterial = effectMaterial;
        }
    }
}
