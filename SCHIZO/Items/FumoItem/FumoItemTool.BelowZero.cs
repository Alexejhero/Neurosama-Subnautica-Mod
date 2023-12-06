using UnityEngine;

namespace SCHIZO.Items.FumoItem;

partial class FumoItemTool
{
    private (Transform parent, Vector3 localPosOffset) GetHugOffset(float distScale)
    {
        Transform player = usingPlayer.transform;
        // TODO: move just the arms instead of the entire body (IK)
        Transform parent = player.Find("body").GetChild(0);

        // to chest (a bit lower than camera/face)
        Vector3 worldDirectionToPlayer = player.position + _chestOffset - transform.position;
        Vector3 worldOffset = Vector3.Slerp(Vector3.zero, worldDirectionToPlayer * _hugDistance, Mathf.Clamp01(distScale));
        Vector3 localOffset = parent.worldToLocalMatrix.MultiplyVector(worldOffset);

        // in BZ the camera is attached to the body
        Transform cam = player.Find("camPivot/camRoot/camOffset/pdaCamPivot");
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
