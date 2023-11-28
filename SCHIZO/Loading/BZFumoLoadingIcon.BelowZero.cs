using Nautilus.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SCHIZO.Loading;

partial class BZFumoLoadingIcon
{
    private void Awake()
    {
        uGUI_Flipbook flipbook = GetComponentInParent<uGUI_Flipbook>();
        if (!flipbook)
        {
            Destroy(this);
            return;
        }
        if (flipbook.target.Exists() is Image im)
        {
            im.sprite = sprite;
            flipbook.cols = 1;
            flipbook.rows = 1;
            flipbook.frameEnd = 0;
        }
    }

    private void Update()
    {
        transform.parent.Rotate(0, 360 * Time.deltaTime, 0);
    }
}
