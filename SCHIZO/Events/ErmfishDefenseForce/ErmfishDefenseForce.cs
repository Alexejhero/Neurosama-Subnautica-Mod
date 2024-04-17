using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UWE;

namespace SCHIZO.Events.ErmfishDefenseForce;

partial class ErmfishDefenseForce
{
    public static ErmfishDefenseForce instance;

    public List<GameObject> ActiveDefenders { get; private set; }
    public float CurrentAggro { get; set; }

    partial class Defender
    {
        private GameObject _prefab;

        public IEnumerator GetPrefab(IOut<GameObject> prefab)
        {
            if (_prefab is null)
            {
                IPrefabRequest request = PrefabDatabase.GetPrefabAsync(ClassId);
                yield return request;

                if (!request.TryGetPrefab(out _prefab))
                {
                    LOGGER.LogError($"(EDF) Could not get prefab for {ClassId}");
                    yield break;
                }
            }
            prefab.Set(_prefab);
        }
    }

    public override bool IsOccurring => _spawning;

    private bool _spawning;
    private float _cooldownTimer;
    public bool debugKarma = false;

    private void Start()
    {
        instance = this;

        ActiveDefenders = [];
        _cooldownTimer = startCooldown / 2f;
    }

    public void OnCook(TechType techType) => OnAggroEvent(techType, cookAggro);
    public void OnEat(TechType techType) => OnAggroEvent(techType, eatAggro);
    public void OnPickup(TechType techType) => OnAggroEvent(techType, pickUpAggro);
    public void OnDrop(TechType techType) => OnAggroEvent(techType, dropAggro);
    public void OnAttack(TechType techType) => OnAggroEvent(techType, attackAggro);
    private void OnAggroEvent(TechType techType, float aggroDelta, [CallerMemberName] string source = null)
    {
        if (!protectedSpecies.Any(p => techType == p.ModItem)) return;

        AddAggro(aggroDelta, $"{source}|{techType}");
    }
    public void AddAggro(float aggro, [CallerMemberName] string source = null)
    {
        CurrentAggro += aggro;
        if (debugKarma)
        {
            ErrorMessage.AddMessage($"({source}) aggro {(aggro >= 0 ? '+' : '-')}{aggro}={CurrentAggro}");
        }
    }

    public void SetAggro(float aggro, [CallerMemberName] string source = null)
    {
        CurrentAggro = aggro;
        if (debugKarma)
        {
            ErrorMessage.AddMessage($"({source}) aggro ={CurrentAggro}");
        }
    }

    internal void Reset()
    {
        _cooldownTimer = 0;
        OnPlayerKilledByDefender(null);
    }

    protected override bool ShouldStartEvent()
    {
        return !_spawning
            && CurrentAggro > startAggroThreshold
            && _cooldownTimer <= Time.time
            && Player.main && !Player.main.currentSub
#if BELOWZERO
            && Player.main.currentInterior is null // don't spawn indoors
#endif
            && Player.main.IsUnderwaterForSwimming() // there are no land kill squads... yet
            ;
    }

    protected override void UpdateLogic()
    {
        for (int i = 0; i < ActiveDefenders.Count; i++)
        {
            GameObject defender = ActiveDefenders[i];
            if (!defender)
            {
                ActiveDefenders.RemoveAtSwapBack(i);
                i--;
                continue;
            }
        }

        if (ActiveDefenders.Count == 0)
            EndEvent();
    }

    protected override void UpdateRender() { }

    private Defender RollRandomDefender()
    {
        float roll = Random.Range(0f, 1f);

        List<Defender> spawnable = defenders
            .Where(d => d.aggroCost < CurrentAggro)
            .ToList();

        float totalWeight = spawnable.Sum(d => d.spawnWeight);
        if (totalWeight == 0)
        {
            LOGGER.LogWarning($"(EDF) No spawnable groups found - {defenders.Length} total defenders ({string.Join(",", defenders.Select(d => d.spawnWeight))}), {CurrentAggro} aggro");
            return null;
        }

        float thresh = 0;
        foreach (Defender def in spawnable)
        {
            thresh += def.spawnWeight / totalWeight;
            if (roll <= thresh)
                return def;
        }
        LOGGER.LogWarning($"(EDF) Did not find defender group to spawn? rolled {roll} vs {thresh}");
        return null;
    }

    private IEnumerator SpawnDefenderGroup(Defender defender)
    {
        if (defender is null)
            yield break;
        int willSpawn = defender.maxGroupSize;
        if (defender.aggroCost > 0)
        {
            int canSpawn = Mathf.FloorToInt(CurrentAggro / defender.aggroCost);
            int wantToSpawn = Random.RandomRangeInt(0, canSpawn);
            willSpawn = Mathf.Min(defender.maxGroupSize, wantToSpawn);
        }
        if (defender.maxGroupSize == 0)
        {
            ErrorMessage.AddMessage($"delopver forgor to set group size on {defender.name} everybody point and laugh");
            yield break;
        }
        _spawning = true;
        TaskResult<GameObject> prefabTask = new();
        yield return defender.GetPrefab(prefabTask);
        GameObject prefab = prefabTask.Get();
        if (!prefab) yield break;
        CurrentAggro -= defender.aggroCost * willSpawn;
        LOGGER.LogDebug($"(EDF) spawning {defender.name} ({willSpawn} {defender.ClassId})");
        for (int i = 0; i < willSpawn; i++)
        {
            GameObject instance = GameObject.Instantiate(prefab);
            // todo check for free space
            //var bounds = new Bounds(instance.transform.position, Vector3.zero);
            //foreach (var collider in instance.GetComponents<Collider>())
            //    bounds.Encapsulate(collider.bounds);

            instance.transform.position = Player.main.transform.position + willSpawn * (Random.onUnitSphere - Player.main.transform.forward);
            ActiveDefenders.Add(instance);
        }
        _spawning = false;
    }

    public override void StartEvent()
    {
        if (IsFirstTime)
            ErrorMessage.AddMessage("Ermfish Defense Force deployed");
        base.StartEvent();
        _cooldownTimer = Time.time + startCooldown;
        
        StartCoroutine(SpawnDefenderGroup(RollRandomDefender()));
    }

    public void OnPlayerKilledByDefender(GameObject defender)
    {
        foreach (GameObject d in ActiveDefenders)
            Destroy(d);
        ActiveDefenders.Clear();
        float reduction = -killedByDefenderAggro;
        AddAggro(-Mathf.Min(reduction, CurrentAggro));
        EndEvent();
    }
}
