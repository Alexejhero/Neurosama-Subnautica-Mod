using UnityEngine;

namespace SCHIZO.HullPlates;

public sealed class TextureHider : MonoBehaviour
{
    private Constructable cons;
    public MeshRenderer rend;

    public void Awake()
    {
        cons = GetComponentInChildren<Constructable>();
    }

    public void OnEnable()
    {
        if (rend) rend.enabled = cons.constructed;
    }

    public void OnDisable()
    {
        if (rend) rend.enabled = cons.constructed;
    }

    public void OnDisable()
    {
        rend.enabled = false;
    }
}
