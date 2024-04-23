using UnityEngine;

namespace SCHIZO.Items.Components;

partial class SeaMonkeyHeldItemOverrides
{
    private Vector3 _savedLocalPos;
    private Quaternion _savedLocalRot;
    public void OnPickedUp()
    {
        if (!enabled) return;

        _savedLocalPos = overrideTransform.localPosition;
        _savedLocalRot = overrideTransform.localRotation;

        overrideTransform.localPosition = localPosition;
        overrideTransform.localEulerAngles = localRotation;
    }

    public void OnDropped()
    {
        if (!enabled) return;

        overrideTransform.localPosition = _savedLocalPos;
        overrideTransform.localRotation = _savedLocalRot;
    }
}
