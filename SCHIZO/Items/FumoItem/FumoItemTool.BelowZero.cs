using UnityEngine;

namespace SCHIZO.Items.FumoItem;

public partial class FumoItemTool
{
    // in BZ, the model is positioned differently
    private readonly record struct LocalTransformData(Vector3 position, Vector3 rotation)
    {
        public void Apply(Transform transform)
        {
            transform.localPosition = position;
            transform.localEulerAngles = rotation;
        }
    }
    private static readonly LocalTransformData _bzModelData = new(new Vector3(0.03f, -0.10f, 0.05f), new Vector3(24,0,343));
    private static readonly LocalTransformData _bzRightIkData = new(new Vector3(-0.1f, 0.2f, 0.25f), new Vector3(60, 210, 135));
    private static readonly LocalTransformData _bzLeftIkData = new(new Vector3(0.18f, 0.12f, 0.28f), new Vector3(300, 60, 200));

    private void FixBZModelTransform()
    {
        Transform vm = GetComponent<FPModel>().viewModel.transform;

        _bzModelData.Apply(vm.Find("neurofumo new"));
        _bzRightIkData.Apply(vm.Find("IK_RightHand"));
        _bzLeftIkData.Apply(vm.Find("IK_LeftHand"));
    }

    private (Transform parent, Vector3 localPosOffset) GetHugOffset(float distScale)
    {
        // TODO: move just the arms instead of the entire body (IK)
        Transform parent = transform.root.Find("body").GetChild(0);

        // to chest (a bit lower than camera/face)
        Vector3 worldDirectionToPlayer = usingPlayer.transform.position + _chestOffset - transform.position;
        Vector3 worldOffset = Vector3.Slerp(Vector3.zero, worldDirectionToPlayer * _hugDistance, Mathf.Clamp01(distScale));
        Vector3 localOffset = parent.worldToLocalMatrix.MultiplyVector(worldOffset);

        // in BZ the camera is attached to the body
        Transform cam = transform.root.Find("camPivot/camRoot/camOffset/pdaCamPivot");
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
