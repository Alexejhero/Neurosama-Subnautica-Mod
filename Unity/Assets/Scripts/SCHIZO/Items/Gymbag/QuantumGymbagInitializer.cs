using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items.Gymbag
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
