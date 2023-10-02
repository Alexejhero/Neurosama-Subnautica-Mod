using System.Collections;
using RuntimeDebugDraw;
using UnityEngine;

namespace SCHIZO.Items.FumoItem;

public sealed partial class FumoItemTool : CustomPlayerTool
{
    private float hugStartTime;
    private const float hugAttackDuration = 0.15f;
    private const float hugReleaseDuration = 0.2f;
    private float hugStopTime;
    private const float hugCooldown = 1f;
    private float nextHugTime;

    private bool isHugging;
    private const float hugDistance = 0.5f;
    private Vector3 localDirectionToPlayer;
    private Vector3 prevHugPosOffset;
    private const float hugMoveSpeedMulti = 0.5f;
    private const int hugColdResistBuff = 25;
    private bool hugEffectApplied;

    private GroundMotor groundMotor;

    public new void Awake()
    {
        animTechType = TechType.Floater;
        hasPrimaryUse = true;
        primaryUseTextLanguageString = "Hug ({0})";
        holsterTime = 0.1f;

        base.Awake();
    }
    public override void OnHolster()
    {
        StopHugging();
        base.OnHolster();
    }

    private IEnumerator DelayShow(bool show, float delay)
    {
        yield return delay == 0 ? null : new WaitForSeconds(delay);
        ToggleShow(show);
    }
    private void ToggleShow(bool show)
    {
        (GetComponent<FPModel>()?.viewModel ?? gameObject).SetActive(show);
    }

    public override bool OnRightHandDown() => false; // don't play floater release anim

    public override bool OnRightHandHeld()
    {
        if (!isHugging && Time.time > nextHugTime)
            StartHugging();
        return base.OnRightHandHeld();
    }

    public override bool OnRightHandUp()
    {
        StopHugging();
        return base.OnRightHandUp();
    }

    public void Update()
    {
        Transform slot = usingPlayer.rightHandSlot;
        Transform handAttach = transform.parent;
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            Draw.DrawRay(transform.position, transform.forward, Color.blue, 5f);
            Draw.DrawRay(transform.position, transform.up, Color.green, 5f);
            Draw.DrawRay(slot.position, slot.forward, Color.red, 5f);
            Draw.DrawRay(slot.position, slot.up, Color.yellow, 5f);
            Draw.DrawRay(handAttach.position, handAttach.forward, Color.cyan, 5f);
            Draw.DrawRay(handAttach.position, handAttach.up, Color.magenta, 5f);
        }

        if (localDirectionToPlayer == default) return;
        float time = Time.time;
        float distScale = isHugging
            ? (time - hugStartTime) / hugAttackDuration
            : 1 - (time - hugStopTime) / hugReleaseDuration;
        Vector3 offset = Vector3.Slerp(Vector3.zero, -localDirectionToPlayer * hugDistance, Mathf.Clamp01(distScale));

        Vector3 delta = offset - prevHugPosOffset;
        var parent = transform.parent.parent.parent;
        parent.localPosition += delta;
        prevHugPosOffset = offset;
        var toLog = $"{parent.localPosition} {offset} {delta}";
        if (toLog.GetHashCode() != prevHash)
        {
            LOGGER.LogWarning(toLog);
            prevHash = toLog.GetHashCode();
        }
    }

    private int prevHash;

    public void StartHugging()
    {
        if (isHugging || !usingPlayer) return;
        isHugging = true;
        hugStartTime = Time.time;
        Vector3 worldDirectionToPlayer = transform.position - usingPlayer.transform.position;
        localDirectionToPlayer = transform.worldToLocalMatrix.MultiplyVector(worldDirectionToPlayer._X0Z());

        if (hugEffectApplied) return;
        ApplyMoveSpeedMulti(hugMoveSpeedMulti);
        ApplyColdResistBuff(hugColdResistBuff);
        hugEffectApplied = true;
    }

    public void StopHugging()
    {
        if (!isHugging || !usingPlayer) return;
        isHugging = false;
        hugStopTime = Time.time;
        nextHugTime = Time.time + hugCooldown;

        if (!hugEffectApplied) return;
        ApplyMoveSpeedMulti(1f/hugMoveSpeedMulti);
        ApplyColdResistBuff(-hugColdResistBuff);
        hugEffectApplied = false;
    }

    private void ApplyMoveSpeedMulti(float multi)
    {
        // a formal apology for the following lines of code can be issued to any Subnautica dev on request
        if (!groundMotor) groundMotor = (usingPlayer !?? Player.main).GetComponent<GroundMotor>();
        groundMotor.forwardMaxSpeed *= multi;
        groundMotor.strafeMaxSpeed *= multi;
        groundMotor.backwardMaxSpeed *= multi;
        usingPlayer.GetComponent<UnderwaterMotor>().debugSpeedMult *= multi;
    }
}
