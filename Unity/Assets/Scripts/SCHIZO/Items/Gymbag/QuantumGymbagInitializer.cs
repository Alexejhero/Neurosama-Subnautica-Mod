using UnityEngine;

namespace SCHIZO.Items.Gymbag
{
    public sealed class QuantumGymbagInitializer : MonoBehaviour
    {
#if BELOWZERO
        private void Awake()
        {
            GetComponentInParent<QuantumLocker>().Start();
        }
#endif
    }
}
