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

    private HashSet<TechType> _techTypes;
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

    public override bool IsOccurring => _spawning || ActiveDefenders.Count > 0;

    private bool _spawning;
    public bool debugKarma;
    public bool debugSpawns;

    protected override void Start()
    {
        instance = this;

        _techTypes = new(protectedSpecies.Select(data => (TechType)data.ModItem));
        ActiveDefenders = [];
    }

    public void OnPickup(TechType techType) => OnAggroEvent(techType, pickUpAggro);
    public void OnAttack(TechType techType) => OnAggroEvent(techType, attackAggro);
    public void OnCook(TechType techType) => OnAggroEvent(techType, cookAggro);
    public void OnEat(TechType techType) => OnAggroEvent(techType, eatAggro);
    public void OnDrop(TechType techType) => OnAggroEvent(techType, 0);
    private void OnAggroEvent(TechType techType, float aggroDelta, [CallerMemberName] string source = null)
    {
        if (!_techTypes.Contains(techType)) return;

        AddAggro(aggroDelta, $"{source}|{techType}");
    }
    public void AddAggro(float delta, [CallerMemberName] string source = null)
    {
        CurrentAggro += delta;
        if (debugKarma)
        {
            LOGGER.LogDebug($"({source}) aggro {(delta >= 0 ? '+' : '-')}{Mathf.Abs(delta)}={CurrentAggro}");
        }
    }

    public void SetAggro(float value, [CallerMemberName] string source = null)
    {
        CurrentAggro = value;
        if (debugKarma)
        {
            LOGGER.LogDebug($"({source}) aggro ={CurrentAggro}");
        }
    }

    internal void Reset()
    {
        OnPlayerKilledByDefender(null);
    }

    protected override bool ShouldStartEvent()
    {
        if (CurrentAggro > 0)
            AddAggro(-decay * Time.deltaTime);

        return !_spawning
            && CurrentAggro > (IsFirstTime ? firstTimeThreshold : spawnThreshold)
            && player && !player.currentSub
#if BELOWZERO
            && player.currentInterior is null // don't spawn indoors
#endif
            && player.IsUnderwaterForSwimming(); // there are no land kill squads... yet
    }

    protected override void UpdateLogic()
    {
        for (int i = 0; i < ActiveDefenders.Count; i++)
        {
            GameObject defender = ActiveDefenders[i];
            if (defender)
            {
                LiveMixin liveMixin = defender.GetComponent<LiveMixin>();
                if (!liveMixin || liveMixin.IsAlive())
                    continue;
            }
            ActiveDefenders.RemoveAtSwapBack(i);
            i--;
        }

        // spawn another squad even if the previous one is still alive
        if (ShouldStartEvent())
            StartEvent();
        else if (ActiveDefenders.Count == 0)
            EndEvent();
    }

    protected override void UpdateRender() { }

    private Defender RollRandomDefender()
    {
        if (defenders.Length == 1)
            return defenders[0];

        float totalWeight = defenders.Sum(d => d.spawnWeight);
        if (totalWeight == 0)
        {
            LOGGER.LogWarning("(EDF) All defenders have 0 weight, aborting spawn");
            return null;
        }

        float roll = Random.Range(0f, 1f);
        float thresh = 0;
        foreach (Defender def in defenders)
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
        if (defender.maxGroupSize == 0)
        {
            ErrorMessage.AddMessage($"delopver forgor to set group size on {defender.ClassId} everybody point and laugh");
            yield break;
        }
        int willSpawn = Random.RandomRangeInt(1, defender.maxGroupSize);
        if (debugSpawns) LOGGER.LogDebug($"(EDF) spawning {willSpawn} {defender.ClassId}");
        _spawning = true;
        TaskResult<GameObject> prefabTask = new();
        yield return defender.GetPrefab(prefabTask);
        GameObject prefab = prefabTask.Get();
        if (!prefab) yield break;
        CurrentAggro = 0;

        // todo check free space
        //Bounds bounds = new();
        //prefab.GetComponentsInChildren<Collider>()
        //    .Where(c => !c.isTrigger)
        //    .ForEach(coll => bounds.Encapsulate(coll.bounds));
        int spawned = 0;
        for (int i = 0; i < willSpawn; i++)
        {
            Vector3 spawnPos = player.transform.position + willSpawn * (Random.onUnitSphere - player.transform.forward);

            // basic "spawning inside a wall" check
            int max = UWE.Utils.RaycastIntoSharedBuffer(player.transform.position, spawnPos);
            // closest blocking ray hit
            RaycastHit blockingHit = UWE.Utils.sharedHitBuffer.Take(max)
                .Where(hit => hit.collider.gameObject != player.gameObject)
                .OrderBy(hit => hit.point.DistanceSqrXZ(player.transform.position))
                .FirstOrDefault();
            if (blockingHit.point != default)
                spawnPos = blockingHit.point + blockingHit.normal * 0.1f;

            GameObject instance = GameObject.Instantiate(prefab);
            instance.transform.position = spawnPos;
            if (debugSpawns)
            {
                if (blockingHit.point != default)
                    LOGGER.LogWarning($"(EDF) spawn raycast blocked by {blockingHit.collider} at {blockingHit.point}");
                GameObject spawnMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                spawnMarker.transform.localScale = new(0.5f, 0.5f, 0.5f);
                spawnMarker.transform.position = spawnPos;
                Destroy(spawnMarker, 10f);
            }
            instance.transform.LookAt(player.transform);

            ActiveDefenders.Add(instance);
            spawned++;

            // don't save
            LargeWorldEntity lwe = instance.GetComponent<LargeWorldEntity>();
            if (lwe) LargeWorldStreamer.main.cellManager.UnregisterEntity(lwe);
        }
        if (debugSpawns) LOGGER.LogDebug($"(EDF) spawned {spawned} {defender.ClassId}");
        _spawning = false;
    }

    public override void StartEvent()
    {
        if (!IsOccurring)
        {
            base.StartEvent();
        }
        ErrorMessage.AddMessage(messages.GetRandom());

        StartCoroutine(SpawnDefenderGroup(RollRandomDefender()));
    }

    public void OnPlayerKilledByDefender(GameObject defender)
    {
        ActiveDefenders.ForEach(Destroy);
        ActiveDefenders.Clear();
        if (CurrentAggro > 0)
            AddAggro(-CurrentAggro);
        EndEvent();
    }
}
