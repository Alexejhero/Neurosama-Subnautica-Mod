using System.Diagnostics.CodeAnalysis;
using Nautilus.Handlers;
using SCHIZO.Extensions;
using UnityEngine;

namespace SCHIZO.Items.FumoItem;

public class FumoItem : UnityPrefab
{
    [SetsRequiredMembers]
    public FumoItem(ModItem modItem) : base(modItem)
    {
    }

    protected override void ModifyPrefab(GameObject prefab)
    {
        Rigidbody rb = prefab.EnsureComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        Pickupable pickupable = prefab.EnsureComponent<Pickupable>();
        WorldForces gravity = prefab.EnsureComponent<WorldForces>();
        FumoItemTool tool = prefab.AddComponent<FumoItemTool>();
        tool.ikAimLeftArm = true;
        tool.ikAimRightArm = true;
        //tool.useLeftAimTargetOnPlayer = true;
        tool.leftHandIKTarget = prefab.transform.Find("VM/IK_LeftHand");
        tool.rightHandIKTarget = prefab.transform.Find("VM/IK_RightHand");
        //tool.mainCollider = prefab.GetComponentInChildren<Collider>();
#if BELOWZERO
        tool.ikAimLookDownAngleLimit = 30f;
#endif
        tool.pickupable = pickupable;
        FPModel fpModel = prefab.EnsureComponent<FPModel>();
        fpModel.propModel = prefab.transform.Find("WM").gameObject;
        fpModel.viewModel = prefab.transform.Find("VM").gameObject;

        prefab.EnsureComponentFields();
    }

    protected override void PostRegister()
    {
#if BELOWZERO
        CraftDataHandler.SetColdResistance(modItem, 20);
#endif
    }
}
