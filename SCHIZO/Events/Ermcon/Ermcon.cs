using System.Collections.Generic;
using System.Linq;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Events.Ermcon;

public partial class Ermcon
{
    public static Ermcon instance;

    public override bool IsOccurring => conMembers.Count > 0;

    public HashSet<ErmconPanelist> targets;
    public ErmconPanelist playerTarget;

    private List<ErmconAttendee> conMembers;
    private float _eventEndTime;
    private bool _hasRolled;

    private const float _minSearchInterval = 1f;
    private float _lastSearchTime;
    public float stareTime;

    private void Start()
    {
        instance = this;
        conMembers = new List<ErmconAttendee>();
        targets = new HashSet<ErmconPanelist>();
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
                    .ForEach(c => c.OnTargetRemoved());
            }
        }

        conMembers = conMembers.Where(m => m).ToList();
    }

    protected override void UpdateRender() { }

    public override void StartEvent()
    {
        // don't search again if autostarting
        List<ErmconAttendee> erms = conMembers.Count > 0 ? conMembers
            : LocateLocalErmaniacs(player.gameObject).ToList(); // manual start
        int totalAttendance = Mathf.Min(MaxAttendance, erms.Count);
        if (totalAttendance <= 0)
        {
            MessageHelpers.WriteCommandOutput($"Nobody wanted to go to Ermcon :(");
            return;
        }
        LOGGER.LogMessage($"The upcoming Ermcon will be visited by {totalAttendance} afficionados");
        conMembers.AddRange(erms.Take(totalAttendance));

        _eventEndTime = Time.time + eventDuration;
        base.StartEvent();
    }

    public override void EndEvent()
    {
        targets.Clear();
        foreach (ErmconAttendee ermEnthusiast in conMembers)
        {
            if (!ermEnthusiast) continue;
            ermEnthusiast.enabled = false;
        }

        _eventEndTime = Time.time;

        conMembers.Clear();
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
