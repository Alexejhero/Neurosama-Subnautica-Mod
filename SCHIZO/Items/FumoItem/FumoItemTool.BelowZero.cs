using UnityEngine;
using RootMotion.FinalIK;
using RuntimeDebugDraw;

namespace SCHIZO.Items.FumoItem;

public partial class FumoItemTool
{
    // in BZ, the holding distance is different
    const float hugDistMulti = 0.5f;

    private (Transform parent, Vector3 localPosOffset) GetHugOffset(float distScale)
    {
        // TODO: move just the arms instead of the entire body
        Transform parent = transform.root.Find("body").GetChild(0);

        // to chest (a bit lower than camera/face)
        Vector3 worldDirectionToPlayer = usingPlayer.transform.position + chestOffset - transform.position;
        Vector3 worldOffset = Vector3.Slerp(Vector3.zero, worldDirectionToPlayer * hugDistance * hugDistMulti, Mathf.Clamp01(distScale));
        Vector3 localOffset = parent.worldToLocalMatrix.MultiplyVector(worldOffset);

        // in BZ the camera moves with the body (fix along with the above todo)
        Transform cam = MainCamera.camera.transform;
        cam.localPosition = cam.worldToLocalMatrix.MultiplyVector(-worldOffset);

        return (parent, localOffset);
    }

    private void ApplyColdResistBuff(int buff)
    {
        if (!usingPlayer) return;
        BodyTemperature bodyTemperature = usingPlayer.GetComponent<BodyTemperature>();
        if (!bodyTemperature) return;
        bodyTemperature.coldResistEquipmentBuff += buff;
    }
}
