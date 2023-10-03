using UnityEngine;

namespace SCHIZO.Items.FumoItem;

public partial class FumoItemTool
{
    private (Transform parent, Vector3 localPosOffset) GetHugOffset(float distScale)
    {
        // TODO: move just the arms instead of the entire body
        Transform parent = transform.root.Find("body").GetChild(0);

        // to chest (a bit lower than camera/face)
        Vector3 worldDirectionToPlayer = usingPlayer.transform.position + chestOffset - transform.position;
        Vector3 localDirectionToPlayer = parent.worldToLocalMatrix.MultiplyVector(worldDirectionToPlayer);
        Vector3 offset = Vector3.Slerp(Vector3.zero, localDirectionToPlayer * hugDistance, Mathf.Clamp01(distScale));

        return (parent, offset);
    }

    private void ApplyColdResistBuff(int _)
    {
    }
}
