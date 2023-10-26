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

    public float boredom;
    public float burnout;
    public float stareTime;

    public ErmconPanelist CurrentTarget { get; private set; }
    private Dictionary<ErmconPanelist, float> _visited;

    public override void Awake()
    {
        base.Awake();
        _visited = new Dictionary<ErmconPanelist, float>();
        // "meta"-priority - this number determines the order in which actions get *evaluated*
        // and the priority obtained from Evaluate() actually determines which action gets *performed*
        evaluatePriority = 99f;
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
        burnout = 0;
        _visited.Clear();
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
        if (burnout > patience)
        {
            StopPerform(time);
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

    /// <returns>A <see cref="bool" /> indicating whether we have an active target.</returns>
    private bool UpdateTarget(float deltaTime)
    {
        if (!CurrentTarget) return SwitchTarget();

        float timeOnTarget = _visited[CurrentTarget];
        float boredomGrowth = deltaTime * _boredomGrowth.Evaluate(timeOnTarget) / (1 + CurrentTarget.entertainmentFactor);
        timeOnTarget += deltaTime;

        boredom += boredomGrowth - patience;
        _visited[CurrentTarget] = timeOnTarget;

        if (boredom > patience)
            return SwitchTarget();
        return true;
    }

    public override void StopPerform(float time)
    {
        enabled = false;
    }

    // priority activity
    public override float Evaluate(float time) => 99f;

    public bool SwitchTarget(ErmconPanelist forceTarget = null)
    {
        CurrentTarget = forceTarget ? forceTarget : PickAnotherBooth();
        if (!CurrentTarget) return false;
        if (!_visited.ContainsKey(CurrentTarget)) _visited[CurrentTarget] = 0;
        swimBehaviour.LookAt(CurrentTarget.transform);
        boredom = 0;
        return true;
    }

    /// <summary>
    /// Targets are picked in this order:<br/>
    /// <list type="number">
    ///   <item>
    ///     Time spent on target (0 if unvisited) divided by entertainment factor (EF), smallest first<br/>
    ///     We divide by the EF because attendees naturally switch away from boring targets quicker
    ///   </item>
    ///   <item>
    ///     EF as the tiebreaker, highest first
    ///   </item>
    /// </list>
    /// </summary>
    private ErmconPanelist PickAnotherBooth()
    {
        return Ermcon.instance.targets
            .OrderBy(t => _visited.GetOrDefault(t, 0f) / t.entertainmentFactor)
            .ThenByDescending(t => t.entertainmentFactor)
            .FirstOrDefault();
    }
}
