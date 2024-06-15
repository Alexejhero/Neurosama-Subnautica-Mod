using UnityEngine;
using UnityEngine.UI;

namespace SCHIZO.SwarmControl
{
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("SCHIZO/SwarmControl/Dead Pixel")]
    public sealed class DeadPixel : MonoBehaviour
    {
        public float duration;
        private void Start()
        {
            Destroy(gameObject, duration);
        }
    }
}