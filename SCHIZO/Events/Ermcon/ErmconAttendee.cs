using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Nautilus.Assets.CustomModelData;

namespace SCHIZO.Events.Ermcon;

public partial class ErmconAttendee : IHandTarget
{
    private static readonly AnimationCurve _boredomGrowth = new(
        new Keyframe(0f, 0f),
        new Keyframe(5f, 0.01f),
        new Keyframe(10f, 0.05f),
        new Keyframe(15f, 0.1f),
        new Keyframe(30f, 0.2f),
        new Keyframe(60f, 0.5f),
        new Keyframe(90f, 2f),
        new Keyframe(120f, 100f)
    );
    private List<(HandTarget target, bool wasEnabled)> _otherHandTargets;

    private float _boredom;
    private float _engagement;
    public float stareTime;

    public ErmconPanelist CurrentTarget { get; private set; }
    private HashSet<ErmconPanelist> _visited;
    private float _timeOnTarget;

    public override void Awake()
    {
        base.Awake();
        _visited = new HashSet<ErmconPanelist>();
    }
    public void OnHandHover(GUIHand hand)
    {
        HandReticle.main.SetText(HandReticle.TextType.Hand, "Cannot pick up", false);
        HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "Inappropriate conduct is forbidden during Ermcon", false);
    }
    public void OnHandClick(GUIHand hand)
    {
        // play "no~"?
    }

    public override void OnEnable()
    {
        _otherHandTargets = GetComponents<HandTarget>().Select(ht => (ht, ht.enabled)).ToList();
        _otherHandTargets.ForEach(pair => pair.target.enabled = false);
        base.OnEnable();
    }

    public void OnDisable()
    {
        creature.ScanCreatureActions();
        creature.GetComponent<SwimBehaviour>().LookForward();
        _otherHandTargets.ForEach(pair => pair.target.enabled = pair.wasEnabled);
    }

    public void OnTargetRemoved()
    {
        CurrentTarget = null;
        StareFor(Random.Range(2.9f, 3.1f));
        swimBehaviour.LookAt(Ermcon.instance.playerTarget.transform);
    }

    public void StareFor(float time)
    {
        if (stareTime < time) stareTime = time;
    }

    // we don't call base.*Perform because they're empty
    public override void StartPerform(float time)
    {
        SwitchTarget();
    }

    public override void Perform(float time, float deltaTime)
    {
        SwimBehaviour swim = swimBehaviour;
        Locomotion loco = swimBehaviour.splineFollowing.locomotion;
        if (stareTime > 0)
        {
            // stop and stare
            loco.ApplyVelocity(-0.5f * loco.useRigidbody.velocity);
            swim.Idle();
            stareTime -= deltaTime;
            return;
        }
        if (!UpdateTarget(deltaTime)) return;

        Vector3 targetPos = CurrentTarget.transform.position;
        if (CurrentTarget.personalSpaceRadius > 0f)
        {
            Vector3 directionToTarget = Vector3.Normalize(targetPos - swim.transform.position);
            targetPos -= directionToTarget * CurrentTarget.personalSpaceRadius;
        }
        float distSqr = swim.transform.position.DistanceSqrXZ(targetPos);
        float swimVelocity = distSqr switch
        {
            > 10000 => 40, // >100m away (take the train)
            > 2500 => 10, // 50m (take the bus)
            > 900 => 4, // 30m (take a taxi)
            > 400 => 2, // 20m (take a bike)
            _ => 1
        };
        swimVelocity *= CurrentTarget.entertainmentFactor;

        swim.SwimTo(targetPos, swimVelocity);
    }

    private bool UpdateTarget(float deltaTime)
    {
        if (!CurrentTarget) return SwitchTarget();

        _boredom += deltaTime * _boredomGrowth.Evaluate(_timeOnTarget) / (1 + CurrentTarget.entertainmentFactor);
        _boredom -= deltaTime * patienceMultiplier;
        _timeOnTarget += deltaTime;
        _engagement += CurrentTarget.entertainmentFactor * deltaTime - _boredom;

        if (_boredom > _engagement)
            return SwitchTarget();
        return true;
    }

    public void OnTargetRemoved(ErmconPanelist panelist, GameObject source)
    {
        if (CurrentTarget == panelist)
        {
            SwitchTarget();
        }
    }

    public override void StopPerform(float time)
    {
        enabled = false;
    }

    // priority activity
    public override float Evaluate(float time) => 99f;

    public bool SwitchTarget(ErmconPanelist forceTarget = null)
    {
        CurrentTarget = forceTarget ? forceTarget : PickAnotherBooth(_visited);
        if (!CurrentTarget) return false;
        _visited.Add(CurrentTarget);
        swimBehaviour.LookAt(CurrentTarget.transform);
        _timeOnTarget = 0;
        _boredom = 0;
        _engagement = startingEngagement;
        return true;
    }

    private ErmconPanelist PickAnotherBooth(ICollection<ErmconPanelist> visited)
    {
        return Ermcon.instance.Targets.FirstOrDefault(t => !visited.Contains(t))
            ?? Ermcon.instance.playerTarget;
    }
}
