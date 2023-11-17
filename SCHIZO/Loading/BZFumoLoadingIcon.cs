using UnityEngine;
using UnityEngine.UI;

namespace SCHIZO.Loading;

partial class BZFumoLoadingIcon
{
    private void Awake()
    {
        if (!GetComponentInParent<SavingIndicator>())
        {
            Destroy(this);
            return;
        }

        uGUI_Flipbook flipbook = GetComponent<uGUI_Flipbook>();
        ((Image) flipbook.target).sprite = sprite;
    }

    private void Update()
    {
        transform.parent.Rotate(0, Time.deltaTime, 0);
    }
}
