using System.Diagnostics;
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
        Vector3 localDirectionToPlayer = parent.worldToLocalMatrix.MultiplyVector(worldDirectionToPlayer);
        Vector3 offset = Vector3.Slerp(Vector3.zero, localDirectionToPlayer * _hugDistance, Mathf.Clamp01(distScale));

        return (parent, offset);
    }

    [Conditional("BELOWZERO")]
    // ReSharper disable once UnusedParameter.Local
    private void ApplyColdResistBuff(int _)
    {
    }
}
