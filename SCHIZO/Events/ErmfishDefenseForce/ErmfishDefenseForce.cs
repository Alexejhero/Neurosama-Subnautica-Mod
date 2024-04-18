using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Nautilus.Json;
using Nautilus.Json.Attributes;
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
    private float _cooldownTimer;
    public bool debugKarma;
    public bool debugSpawns;

    private void Start()
    {
        instance = this;

        _techTypes = new(protectedSpecies.Select(data => (TechType)data.ModItem));
        ActiveDefenders = [];
        _cooldownTimer = startCooldown / 2f;

        SaveData.Instance.Attach(this);
        SaveData.Instance.Load();
    }

    private new void OnDestroy()
    {
        SaveData.Instance.Save();
        SaveData.Instance.Detach();
        base.OnDestroy();
    }

    public void OnCook(TechType techType)
    {
        OnDrop(techType); // losing inventory items doesn't trigger OnDrop
        OnAggroEvent(techType, cookAggro);
    }

    public void OnEat(TechType techType) => OnAggroEvent(techType, eatAggro);
    public void OnPickup(TechType techType) => OnAggroEvent(techType, pickUpAggro);
    public void OnDrop(TechType techType) => OnAggroEvent(techType, dropAggro);
    public void OnAttack(TechType techType) => OnAggroEvent(techType, attackAggro);
    private void OnAggroEvent(TechType techType, float aggroDelta, [CallerMemberName] string source = null)
    {
        if (!_techTypes.Contains(techType)) return;

        AddAggro(aggroDelta, $"{source}|{techType}");
    }
    public void AddAggro(float aggro, [CallerMemberName] string source = null)
    {
        CurrentAggro += aggro;
        if (debugKarma)
        {
            LOGGER.LogDebug($"({source}) aggro {(aggro >= 0 ? '+' : '-')}{Mathf.Abs(aggro)}={CurrentAggro}");
        }
    }

    public void SetAggro(float aggro, [CallerMemberName] string source = null)
    {
        CurrentAggro = aggro;
        if (debugKarma)
        {
            LOGGER.LogDebug($"({source}) aggro ={CurrentAggro}");
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
            && player && !player.currentSub
#if BELOWZERO
            && player.currentInterior is null // don't spawn indoors
#endif
            && player.IsUnderwaterForSwimming() // there are no land kill squads... yet
            ;
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
        if (debugSpawns) LOGGER.LogDebug($"(EDF) spawning {defender.name} ({willSpawn} {defender.ClassId})");
        _spawning = true;
        TaskResult<GameObject> prefabTask = new();
        yield return defender.GetPrefab(prefabTask);
        GameObject prefab = prefabTask.Get();
        if (!prefab) yield break;
        CurrentAggro -= defender.aggroCost * willSpawn;

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
                spawnPos = blockingHit.point;
            
            GameObject instance = GameObject.Instantiate(prefab);
            instance.transform.position = spawnPos;
            if (debugSpawns)
            {
                if (blockingHit.point != default)
                    LOGGER.LogWarning($"spawn raycast blocked by {blockingHit.collider} at {blockingHit.point}");
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
            if (IsFirstTime)
                ErrorMessage.AddMessage("Ermfish Defense Force deployed");
            base.StartEvent();
        }
        _cooldownTimer = Time.time + startCooldown;
        
        StartCoroutine(SpawnDefenderGroup(RollRandomDefender()));
    }

    public void OnPlayerKilledByDefender(GameObject defender)
    {
        ActiveDefenders.ForEach(Destroy);
        ActiveDefenders.Clear();
        float reduction = -killedByDefenderAggro;
        AddAggro(-Mathf.Min(reduction, CurrentAggro));
        EndEvent();
    }

    [FileName(nameof(ErmfishDefenseForce))]
    private class SaveData : SaveDataCache
    {
        private static SaveData _instance;
        public static SaveData Instance => _instance ??= new();

        public float aggro;

        private ErmfishDefenseForce _source;
        public SaveData()
        {
            _instance = this;
        }
        public void Attach(ErmfishDefenseForce source)
        {
            _source = source;
            OnStartedSaving += SaveAggro;
            OnFinishedLoading += LoadAggro;
        }
 
        private void SaveAggro(object sender, JsonFileEventArgs e) => aggro = _source ? _source.CurrentAggro : default;
        private void LoadAggro(object sender, JsonFileEventArgs e) => _source!?.SetAggro(aggro);

        public void Detach()
        {
            OnStartedSaving -= SaveAggro;
            OnFinishedLoading -= LoadAggro;
        }
    }
}
