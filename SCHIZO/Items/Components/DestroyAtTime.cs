using UnityEngine;

namespace SCHIZO.Items.Components;
internal class DestroyAtTime : MonoBehaviour
{
    public float time = float.PositiveInfinity;
    private void Update()
    {
        if (Time.time > time)
            Destroy(gameObject);
    }
}
