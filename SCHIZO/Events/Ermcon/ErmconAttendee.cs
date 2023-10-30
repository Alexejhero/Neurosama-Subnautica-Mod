using System.Collections.Generic;
using System.Linq;
using Nautilus.Utility;
using UnityEngine;

namespace SCHIZO.Events.Ermcon;

public partial class ErmconAttendee : IHandTarget
{
    private static readonly AnimationCurve _boredomGrowth = new(
        new Keyframe(0f, 0f),
        new Keyframe(5f, 0.05f),
        new Keyframe(10f, 0.1f),
        new Keyframe(15f, 0.25f),
        new Keyframe(30f, 1f),
        new Keyframe(60f, 5f),
        new Keyframe(90f, 20f),
        new Keyframe(120f, 1000f)
    );
    private List<(HandTarget target, bool wasEnabled)> _otherHandTargets;

    /// <summary>
    /// A value that increases the longer the creature is focused on one target.<br/>
    /// Once the value surpasses the creature's <see cref="patience">patience</see>, it switches targets.
    /// </summary>
    public float boredom;
    /// <summary>
    /// Like <see cref="boredom">boredom</see>, but for the whole <see cref="Ermcon"/>.<br/>
    /// Once the value surpasses the creature's <see cref="patience">patience</see>, it stops participating in the event.
    /// </summary>
    public float burnout;
    /// <summary>
    /// While this is above zero, the creature will stop and stare at the <see cref="StareTarget">StareTarget</see>.<br/>
    /// Use <see cref="StareAt(Transform, float)"/> to set this.
    /// </summary>
    public float StareTime { get; private set; }
    public Transform StareTarget { get; private set; }

    public ErmconPanelist CurrentTarget { get; private set; }
    private Dictionary<ErmconPanelist, float> _visited;

    public int _verbose;
    private float _savedPatience;

    public override void Awake()
    {
        base.Awake();
        _savedPatience = patience;
        _visited = new Dictionary<ErmconPanelist, float>();
        // "meta"-priority - this number determines the order in which actions get *evaluated*
        // and the priority obtained from Evaluate() actually determines which action gets *performed*
        evaluatePriority = 99f;
        pickupDeniedSounds = pickupDeniedSounds.Initialize(AudioUtils.BusPaths.UnderwaterCreatures);
    }
    public void OnHandHover(GUIHand hand)
    {
        HandReticle.main.SetText(HandReticle.TextType.Hand, "Cannot pick up", false);
        HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "Inappropriate conduct is forbidden during Ermcon", false);
    }
    public void OnHandClick(GUIHand hand)
    {
        pickupDeniedSounds!?.Play(emitter);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        patience = _savedPatience * Random.Range(0.8f, 1.2f);
        if (Ermcon.instance.IsFirstTime) patience *= 1.5f;
        LogSelf($"enabled\npatience: {patience}");
        creature.actions.Clear();
        creature.actions.Add(this);
        _otherHandTargets = GetComponents<HandTarget>().Select(ht => (ht, ht.enabled)).ToList();
        _otherHandTargets.ForEach(pair => pair.target.enabled = false);
    }

    public void OnDisable()
    {
        LogSelf("disabled");
        burnout = 0;
        _visited.Clear();
        creature.ScanCreatureActions();
        creature.GetComponent<SwimBehaviour>().LookForward();
        _otherHandTargets.ForEach(pair => pair.target.enabled = pair.wasEnabled);
    }

    public void OnTargetRemoved(GameObject removedBy)
    {
        float time = Random.Range(2.5f, 3.5f);
        LogSelf($"target removed by {removedBy}, will stare for {time}");
        StareAt(removedBy.transform, time);
    }

    public void StareAt(Transform target, float time)
    {
        StareTime = time;
        StareTarget = target;
    }

    // we don't call base.*Perform because they're empty
    public override void StartPerform(float time)
    {
        LogSelf("StartPerform");
        //SwitchTarget();
    }

    public override void Perform(float time, float deltaTime)
    {
        SwimBehaviour swim = swimBehaviour;
        Locomotion loco = swimBehaviour.splineFollowing.locomotion;
        if (StareTime > 0)
        {
            LogSelf($"staring ({StareTime} left)", 2);
            // stop and stare
            loco.ApplyVelocity(-0.5f * loco.useRigidbody.velocity);
            swim.Idle();
            swim.LookAt(StareTarget);
            StareTime -= deltaTime;
            return;
        }
        if (burnout > patience)
        {
            LogSelf($"stop burnout ({burnout}>{patience})");
            StopPerform(time);
            return;
        }
        if (!UpdateTarget(deltaTime)) return;

        Vector3 targetPos = CurrentTarget.transform.position;
        Vector3 toTarget = targetPos - swim.transform.position;
        float distSqr = toTarget.sqrMagnitude;

        if (distSqr > 400) boredom = 0f; // still travelling

        float swimVelocity = distSqr switch
        {
            > 10000 => 40, // >100m away (take a plane)
            > 2500 => 10, // 50m (take the train)
            > 900 => 4, // 30m (take a bus)
            > 400 => 2, // 20m (take a bike)
            _ => 1
        };
        swimVelocity *= Mathf.Max(1, CurrentTarget.entertainmentFactor);

        Vector3 directionToTarget = Vector3.Normalize(toTarget);
        targetPos -= directionToTarget * CurrentTarget.personalSpaceRadius;

        swim.SwimTo(targetPos, swimVelocity);
        swim.LookAt(CurrentTarget.transform);
    }

    /// <returns>A <see cref="bool" /> indicating whether we have an active target.</returns>
    private bool UpdateTarget(float deltaTime)
    {
        if (!CurrentTarget)
        {
            LogSelf("no target");
            return SwitchTarget();
        }
        Constructable con = CurrentTarget.GetComponent<Constructable>();
        if (con && con.amount < 1)
        {
            LogSelf("target deconstructed");
            return SwitchTarget();
        }

        float timeOnTarget = _visited.GetOrDefault(CurrentTarget, 0f);
        float boredomGrowth = deltaTime * _boredomGrowth.Evaluate(timeOnTarget) / (1 + CurrentTarget.entertainmentFactor);
        LogSelf($"""
            deltaTime: {deltaTime}
            timeOnTarget: {timeOnTarget}
            boredom: {boredom}
            boredomGrowth: {boredomGrowth}
            burnout: {burnout}
            """, 2);
        timeOnTarget += deltaTime;

        boredom += boredomGrowth;
        burnout += boredomGrowth / patience;
        _visited[CurrentTarget] = timeOnTarget;

        if (boredom > patience)
        {
            LogSelf($"switching {boredom}>{patience} (away from {CurrentTarget})");
            return SwitchTarget();
        }
        return true;
    }

    public override void StopPerform(float time)
    {
        LogSelf("StopPerform");
        enabled = false;
    }

    // priority activity
    public override float Evaluate(float time) => 99f;

    public bool SwitchTarget(ErmconPanelist forceTarget = null)
    {
        CurrentTarget = forceTarget ? forceTarget : PickAnotherBooth();
        if (!CurrentTarget)
        {
            LogSelf("could not find a target!");
            return false;
        }
        string msg = $"switching to {CurrentTarget}";
        if (!_visited.ContainsKey(CurrentTarget))
        {
            msg += " (first time)";
            _visited[CurrentTarget] = 0;
            burnout = Mathf.Max(0, burnout - patience/10);
        }
        LogSelf(msg);
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
    ///   <item>EF as the tiebreaker, highest first</item>
    ///   <item>Same time and same EF --> pick random</item>
    /// </list>
    /// </summary>
    private ErmconPanelist PickAnotherBooth()
    {
        return Ermcon.instance.targets
            .GroupBy(t => (timeFactor: _visited.GetOrDefault(t, 0f) / t.entertainmentFactor, t.entertainmentFactor))
            .OrderBy(group => group.Key.timeFactor)
            .ThenByDescending(group => group.Key.entertainmentFactor)
            .FirstOrDefault()?.ToList().GetRandom();
    }

    public void CycleDebug(int? forceLevel = null)
    {
        _verbose = forceLevel ?? (++_verbose % 3);
        if (_verbose == 0) LogSelf("stopped logging", 0);
        else LogSelf("started logging at level " + _verbose);

        GameObject obj = gameObject;
        FPModel fpModel = GetComponent<FPModel>();
        if (fpModel && fpModel.propModel)
            obj = fpModel.propModel;
        obj.GetComponentsInChildren<Renderer>()
            .ForEach(r => r.material.color = _verbose switch
            {
                1 => Color.red,
                2 => Color.green,
                _ => Color.white,
            });
    }
    private void LogSelf(string text, int level = 1)
    {
        if (_verbose >= level) LOGGER.LogWarning($"{GetInstanceID()} {text}");
    }
}
