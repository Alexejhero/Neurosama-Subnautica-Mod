using System.Collections.Generic;
using System.Linq;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Events.Ermcon;

/// <summary>
/// The gathering of Erm enthusiasts from all over.<br/>
/// When it starts, between <see cref="MinAttendance">MinAttendance</see> and <see cref="MaxAttendance">MaxAttendance</see> <see cref="ErmconAttendee"/>s will begin gathering around the player.<br/>
/// Throughout the event, Erm aficionados will pick <see cref="ErmconPanelist"/>s to crowd around.<br/>
/// Erm lovers get bored of staring at the same thing and will switch targets every now and then, staying longer based on the target's <see cref="ErmconPanelist.entertainmentFactor"/>.<br/>
/// If there's not enough entertainment to go around, Erm enjoyers may <see cref="ErmconAttendee.burnout">burn out</see> and leave the event early. Make sure to provide the best environment for your Erms so they can thrive.
/// </summary>
public partial class Ermcon
{
    public static Ermcon instance;

    public override bool IsOccurring => _isOccurring;
    private bool _isOccurring;

    public HashSet<ErmconPanelist> targets;
    public ErmconPanelist playerTarget;

    private List<ErmconAttendee> conMembers;
    private float _eventEndTime;
    private bool _hasRolled;

    private const float _minSearchInterval = 1f;
    private float _lastSearchTime;
    public float stareTime;

    protected override void Start()
    {
        instance = this;
        conMembers = [];
        targets = [];
        // let's not wait the whole cooldown on load
        _eventEndTime = -cooldown / 2;

        player.playerDeathEvent.AddHandler(this, _ => EndEvent());
        playerTarget = player.gameObject.EnsureComponent<ErmconPanelist>();
        playerTarget.entertainmentFactor = 0.5f;
    }

    protected override bool ShouldStartEvent()
    {
        float sinceLastEvent = Time.time - _eventEndTime;
        if (sinceLastEvent < cooldown)
            return false;
        // roll every 6 in-game hours (5min real time)
        // should average out to about one con every 1h-1h30m real time
        // since it's always centered on (or around) the player, it's not a big problem that it's rare
        if (DayNightUtils.dayScalar % 0.25 < 0.01)
        {
            if (_hasRolled) return false;

            _hasRolled = true;
            float chance = 0.1f * (sinceLastEvent / cooldown);
            float roll = Random.Range(0f, 1f);

            if (roll > chance) return false;
        }
        else
        {
            _hasRolled = false;
            return false;
        }

        List<ErmconAttendee> ermsInRange = LocateLocalErmaniacs(player.gameObject).ToList();
        if (ermsInRange.Count < MinAttendance)
        {
            //LOGGER.LogInfo($"Rolled for Ermcon event but only had {ermsInRange.Count} erms, unlucky");
            return false;
        }

        conMembers = ermsInRange;
        return true;
    }

    protected override void UpdateLogic()
    {
        float time = Time.fixedTime;
        if (time > _eventEndTime)
        {
            EndEvent();
            return;
        }

        if (time > _lastSearchTime + _minSearchInterval * (1 + targets.Count))
        {
            targets.AddRange(InviteInfluencers(gameObject));
            _lastSearchTime = time;
        }

        // untarget any deconstructed
        List<Constructable> constructables = targets.Select(c => c.gameObject).SelectComponent<Constructable>().ToList();
        foreach (Constructable con in constructables)
        {
            if (con.constructedAmount < 0.90f) // small buffer to prevent spam
            {
                ErmconPanelist panelist = con.GetComponent<ErmconPanelist>();
                targets.Remove(panelist);
                conMembers.Where(c => c.CurrentTarget == panelist)
                    .ForEach(c => c.OnTargetRemoved(player.gameObject));
            }
        }

        conMembers = conMembers.Where(m => m && m.enabled).ToList();
        if (conMembers.Count == 0) EndEvent();
    }

    protected override void UpdateRender() { }

    public override void StartEvent()
    {
        List<ErmconAttendee> erms = conMembers.Count > 0 ? conMembers    // auto start - use the search we just did in ShouldStartEvent
            : LocateLocalErmaniacs(player.gameObject).ToList();          // manual start
        int totalAttendance = Mathf.Min(MaxAttendance, erms.Count);
        if (totalAttendance <= 0)
        {
            MessageHelpers.WriteCommandOutput($"Nobody wanted to go to Ermcon :(");
            return;
        }
        LOGGER.LogMessage($"The upcoming Ermcon will be visited by {totalAttendance} afficionados");
        conMembers = erms.Take(totalAttendance).ToList();
        conMembers.ForEach(erm => erm.enabled = true);

        _eventEndTime = Time.time + maxDuration;
        _isOccurring = true;
        base.StartEvent();
    }

    public override void EndEvent()
    {
        if (!_isOccurring) return;
        targets.Clear();
        foreach (ErmconAttendee ermEnthusiast in conMembers)
        {
            if (ermEnthusiast) ermEnthusiast.enabled = false;
        }
        conMembers.Clear();

        _eventEndTime = Time.time;
        _isOccurring = false;
        base.EndEvent();
    }

    private IEnumerable<ErmconAttendee> LocateLocalErmaniacs(GameObject center)
    {
        return PhysicsHelpers.ObjectsInRange(center, attendeeSearchRadius)
            .SelectComponentInParent<ErmconAttendee>();
    }

    private IEnumerable<ErmconPanelist> InviteInfluencers(GameObject center)
    {
        return PhysicsHelpers.ObjectsInRange(center, panelistSearchRadius)
            .SelectComponentInParent<ErmconPanelist>()
            .Where(x =>
            {
                if (targets.Contains(x)) return false;
                Constructable con = x.GetComponent<Constructable>();
                return !con || con.constructedAmount == 1;
            }).OrderByDistanceTo(center.transform.position);
    }
}
