using ProtoBuf;
using SCHIZO.Creatures.Tutel;

namespace SCHIZO.Creatures.Ermshark;

/// <summary>Adapted from <see cref="CollectShiny"/></summary>
[ProtoContract]
[RequireComponent(typeof(SwimBehaviour))]
public class BullyTutel : RetargetCreatureAction, IProtoTreeEventListener
{
    [AssertNotNull]
    public Transform mouth;
    [AssertNotNull]
    public Transform tutelAttach;
    private Creature tutel;
    private bool targetPickedUp;
    private float timeNextFindTutel;
    public float updateInterval = 2f;
    public float updateTargetInterval = 1f;
    private float timeNextUpdate;
    private EcoRegion.TargetFilter isTargetValidFilter;
    public float swimVelocity = ErmsharkPrefab.swimVelocity;

    public override void Awake()
    {
        base.Awake();
        isTargetValidFilter = IsTargetValid;
    }

    public override float Evaluate(float time)
    {
        if (timeNextFindTutel < time)
        {
            UpdateBullyTarget();
            if (tutel)
            {
                creature.Aggression.Value = 0;
                creature.Curiosity.Value = 10;
            }
            timeNextFindTutel = time + updateTargetInterval * (1f + 0.2f * Random.value);
        }
        return tutel && tutel.gameObject.activeInHierarchy ? GetEvaluatePriority() : 0f;
    }

    public override void StopPerform(float time)
    {
        DropTutel();
    }

    public void TryPickupTutel(GetCarried getCarried = null)
    {
        getCarried ??= tutel!?.GetComponent<GetCarried>();
        if (getCarried && getCarried.gameObject && getCarried.gameObject.activeInHierarchy)
        {
            if (getCarried.GetComponentInParent<Player>())
            {
                // in player's inventory
                DropTutel();
                timeNextFindTutel = Time.time + 6f;
                return;
            }
            UWE.Utils.SetCollidersEnabled(getCarried.gameObject, false);
            getCarried.transform.parent = tutelAttach;
            getCarried.transform.localPosition = Vector3.zero;
            getCarried.OnPickedUp();
            getCarried.GetComponent<SwimBehaviour>().Idle();
            targetPickedUp = true;
            UWE.Utils.SetIsKinematic(getCarried.GetComponent<Rigidbody>(), true);
            UWE.Utils.SetEnabled(getCarried.GetComponent<LargeWorldEntity>(), false);
            swimBehaviour.SwimTo(transform.position + Vector3.up + 5f * Random.onUnitSphere, Vector3.up, swimVelocity);
            timeNextUpdate = Time.time + 1f;
        }
    }

    private void DropTutel()
    {
        if (tutel && targetPickedUp)
        {
            DropTutelTarget(tutel.gameObject);
            tutel.GetComponent<GetCarried>()?.OnDropped();
        }
        tutel = null;
        targetPickedUp = false;
    }

    private void DropTutelTarget(GameObject target)
    {
        target.transform.parent = null;
        UWE.Utils.SetCollidersEnabled(target, true);
        UWE.Utils.SetIsKinematic(target.GetComponent<Rigidbody>(), false);
        if (target.GetComponent<LargeWorldEntity>() is { } lwe)
            LargeWorldStreamer.main!?.cellManager.RegisterEntity(lwe);
    }

    private static bool IsTargetValid(IEcoTarget target) => TutelLoader.TutelTechTypes.Contains(CraftData.GetTechType(target.GetGameObject()));

    private void UpdateBullyTarget()
    {
        IEcoTarget ecoTarget = EcoRegionManager.main?.FindNearestTarget(EcoTargetType.Coral, transform.position, isTargetValidFilter, 1);
        Creature newTarget = ecoTarget?.GetGameObject().GetComponent<Creature>();
        if (!newTarget) return;

        Vector3 direction = newTarget.transform.position - transform.position;
        float dist = direction.magnitude - 0.5f;
        if (dist > 0 && Physics.Raycast(transform.position, direction, dist, Voxeland.GetTerrainLayerMask()))
            return;
        if (tutel == newTarget || !newTarget.GetComponent<Rigidbody>() || !newTarget.GetComponent<Pickupable>())
            return;
        if (tutel)
        {
            float sqrDistToNew = direction.sqrMagnitude;
            float sqrDistToCurr = (tutel.transform.position - transform.position).sqrMagnitude;
            if (sqrDistToNew > sqrDistToCurr)
            {
                DropTutel();
            }
        }
        tutel = newTarget;
    }

    public override void Perform(float time, float deltaTime)
    {
        if (!tutel) return;
        if (!targetPickedUp)
        {
            if (time > timeNextUpdate)
            {
                timeNextUpdate = time + updateInterval;
                swimBehaviour.SwimTo(tutel.transform.position, -Vector3.up, swimVelocity);
            }
            if ((transform.position - tutel.transform.position).sqrMagnitude < 9f)
            {
                TryPickupTutel();
                return;
            }
        }
        else
        {
            if (tutel.transform.parent != tutelAttach)
            {
                if (tutel.transform.parent && tutel.transform.parent.GetComponentInParent<Creature>())
                {
                    // someone else snatched him
                    targetPickedUp = false;
                    tutel = null;
                }
                else
                {
                    TryPickupTutel();
                }
            }
            if (time > timeNextUpdate)
            {
                timeNextUpdate = time + updateInterval;
                swimBehaviour.SwimTo(transform.position + 2f * swimVelocity * Random.insideUnitSphere, swimVelocity);
                if (Random.value < 0.15f) DropTutel();
            }
            creature.Happy.Add(deltaTime);
            creature.Friendliness.Add(deltaTime);
        }
    }

    private void OnDisable()
    {
        DropTutel();
    }


    public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
    {
    }

    public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
    {
        foreach (Transform trans in tutelAttach)
            DropTutelTarget(trans.gameObject);
    }
}
