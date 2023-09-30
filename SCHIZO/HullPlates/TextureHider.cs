using UnityEngine;

namespace SCHIZO.HullPlates;

public sealed class TextureHider : MonoBehaviour
{
    public MeshRenderer rend;

    public void OnEnable()
    {
        if (rend) rend.enabled = true;
    }

    public void OnDisable()
    {
        if (rend) rend.enabled = false;
    }
}
