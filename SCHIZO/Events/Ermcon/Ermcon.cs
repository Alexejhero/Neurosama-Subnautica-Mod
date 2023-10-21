using System.Collections.Generic;
using System.Linq;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Events.Ermcon;

public partial class Ermcon
{
    public static Ermcon instance;

    public override bool IsOccurring => ConMembers.Count > 0;

    public bool Hivemind;
    public HashSet<ErmconPanelist> Targets;
    public ErmconPanelist playerTarget;

    private List<ErmconAttendee> ConMembers;
    private float _eventStartTime;
    private bool _hasRolled;

    private const float _minSearchInterval = 1f;
    private float _lastSearchTime;
    public float stareTime;

    private void Start()
    {
        instance = this;
        ConMembers = new List<ErmconAttendee>();
        Targets = new HashSet<ErmconPanelist>();
        // let's not wait the whole cooldown on load
        _eventStartTime = -cooldown / 2;

        LOGGER.LogWarning($"{player} {GetComponent<GameEventsConfig>()}");
        player.playerDeathEvent.AddHandler(this, _ => EndEvent());
        playerTarget = player.gameObject.EnsureComponent<ErmconPanelist>();
        playerTarget.entertainmentFactor = 0.5f;
    }

    protected override bool ShouldStartEvent()
    {
        float sinceLastEvent = Time.time - (_eventStartTime + eventDuration);
        if (sinceLastEvent < cooldown)
            return false;
        // roll every 6 in-game hours (5min real time)
        // should average out to about one con every 1h-1h30m real time
        // since it's always centered on (or around) the player, it's not a big problem that it's rare
        if (DayNightUtils.dayScalar % 0.25 < 0.01)
        {
            if (_hasRolled)
                return false;
            _hasRolled = true;
            float chance = 0.1f * (sinceLastEvent / cooldown);
            float roll = Random.Range(0f, 1f);
            if (roll > chance)
            {
                // LOGGER.LogDebug($"roll failed {roll}>{chance}");
                return false;
            }
        }
        else
        {
            _hasRolled = false;
            return false;
        }

        int ermsInRange = PhysicsHelpers.ObjectsInRange(gameObject, attendeeSearchRadius)
            .WithComponent<ErmconAttendee>()
            .Count();
        if (ermsInRange < minAttendance)
        {
            //LOGGER.LogDebug($"Rolled for Ermcon event but only had {ermsInRange} erms, unlucky");
            return false;
        }

        return true;
    }
    protected override void UpdateLogic()
    {
        float time = Time.fixedTime;
        if (time > _eventStartTime + eventDuration)
        {
            EndEvent();
            return;
        }

        if (time > _lastSearchTime + _minSearchInterval * (1 + Targets.Count))
        {
            Targets.AddRange(InviteInfluencers(gameObject));
            _lastSearchTime = time;
        }

        // untarget any deconstructed
        List<Constructable> constructables = Targets.Select(c => c.gameObject).SelectComponent<Constructable>().ToList();
        foreach (Constructable con in constructables)
        {
            if (con.constructedAmount < 0.90f) // small buffer to prevent spam
            {
                ErmconPanelist panelist = con.GetComponent<ErmconPanelist>();
                Targets.Remove(panelist);
                ConMembers.Where(c => c.CurrentTarget == panelist)
                    .ForEach(c => c.OnTargetRemoved());
            }
        }

        ConMembers = ConMembers.Where(m => m).ToList();
    }

    protected override void UpdateRender() { }

    public override void StartEvent()
    {
        List<ErmconAttendee> withinRadius = PhysicsHelpers.ObjectsInRange(gameObject, attendeeSearchRadius)
            .OrderByDistanceTo(player.transform.position)
            .SelectComponentInParent<ErmconAttendee>() // InParent because collision gets WM
            .ToList();
        int totalAttendance = Mathf.Min(maxAttendance, withinRadius.Count);
        LOGGER.LogMessage($"{totalAttendance} congoers will be attending the Ermcon");
        for (int i = 0; i < totalAttendance; i++)
        {
            ErmconAttendee ermconAficionado = withinRadius[i];
            ConMembers.Add(ermconAficionado);
        }

        _eventStartTime = Time.time;
        base.StartEvent();
    }

    public override void EndEvent()
    {
        Targets.Clear();
        foreach (ErmconAttendee ermEnthusiast in ConMembers)
        {
            if (!ermEnthusiast) continue;
            ermEnthusiast.enabled = false;
        }

        ConMembers.Clear();
        base.EndEvent();
    }

    private IEnumerable<ErmconPanelist> InviteInfluencers(GameObject center)
    {
        return PhysicsHelpers.ObjectsInRange(center, panelistSearchRadius)
            .SelectComponentInParent<ErmconPanelist>()
            .Where(x =>
            {
                if (Targets.Contains(x)) return false;
                Constructable con = x.GetComponent<Constructable>();
                return !con || con.constructedAmount == 1;
            }).OrderByDistanceTo(center.transform.position);
    }
}
