using UnityEngine;

namespace SCHIZO.Items.GreenScreen
{
    [AddComponentMenu("SCHIZO/Items/Green Screen")]
    public sealed partial class GreenScreen : MonoBehaviour
    {
        public void Awake()
        {
            GetComponent<MeshRenderer>().material.color = Color.green;
        }
    }
}