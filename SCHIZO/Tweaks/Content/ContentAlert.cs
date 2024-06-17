using UnityEngine;

namespace SCHIZO.Tweaks.Content;

partial class ContentAlert
{
    private void Start()
    {
        GameObject.Instantiate(prefab, transform);
    }
}
