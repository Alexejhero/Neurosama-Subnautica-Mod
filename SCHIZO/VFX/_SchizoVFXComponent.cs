using UnityEngine;

namespace SCHIZO.VFX
{
    public partial class SchizoVFXComponent
    {
        VFXEffect effect;

        public void Awake()
        {
            effect = Camera.main.gameObject.AddComponent<VFXEffect>();
            effect.effectMaterial = effectMaterial;
        }

        private void OnEnable()
        {
            effect.enabled = true;
        }

        private void OnDisable()
        {
            effect.enabled = false;
        }
    }
}
