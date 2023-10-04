using System.Collections.Generic;
using ECCLibrary;
using ECCLibrary.Data;
using HarmonyLib;
using SCHIZO.API.Helpers;
using UnityEngine;

namespace SCHIZO.API.Creatures;

public abstract class CustomCreaturePrefab<TCreature> : CreatureAsset, ICustomCreaturePrefab where TCreature : Creature
{
    public ModItem ModItem { get; }

    protected readonly GameObject creaturePrefab;

    #region ECCLibrary properties

    protected float MaxHealth { get; init; }

    /// <summary>
    /// Roughly determines how far this creature can be loaded in.
    /// </summary>
    protected LargeWorldEntity.CellLevel CellLevel { get; init; } = LargeWorldEntity.CellLevel.Medium;

    /// <summary>
    /// Physic material used for all colliders. If unassigned, will default to <see cref="P:ECCLibrary.ECCUtility.FrictionlessPhysicMaterial" />.
    /// </summary>
    protected PhysicMaterial PhysicMaterial { get; init; }

    /// <summary>
    /// Contains data pertaining to creating the <see cref="T:Locomotion" /> component.
    /// </summary>
    protected LocomotionData LocomotionData { get; init; } = new();

    /// <summary>
    /// Contains data pertaining to creating the <see cref="T:SwimBehaviour" /> component.
    /// </summary>
    protected SwimBehaviourData SwimBehaviourData { get; init; } = new();

    /// <summary>
    /// Contains data pertaining to the <see cref="T:AnimateByVelocity" /> component. This component sets animation parameters based on the creature's direction &amp; velocity.
    /// <br /> Means the 'speed' parameter can be used in the creature's Animator.
    /// <br /> NOT assigned by default!
    /// </summary>
    protected AnimateByVelocityData AnimateByVelocityData { get; init; } = null;

    /// <summary>
    /// Contains data pertaining to the <see cref="T:SwimRandom" /> action. Assigned a generic value by default, but can be changed or set to null.
    /// </summary>
    protected SwimRandomData SwimRandomData { get; init; } = new(0.2f, 3f, Vector3.one * 20f);

    /// <summary>
    /// Contains data pertaining to the <see cref="T:StayAtLeashPosition" /> action. This component keeps creatures from wandering too far. Not assigned by default.
    /// </summary>
    protected StayAtLeashData StayAtLeashData { get; init; }

    /// <summary>
    /// Contains data pertaining to the <see cref="T:FleeWhenScared" /> action. Not assigned by default.
    /// </summary>
    protected FleeWhenScaredData FleeWhenScaredData { get; init; }

    /// <summary>
    /// Contains data pertaining to the <see cref="T:FleeOnDamage" /> action. Assigned by default with default values and a priority of 0.8f.
    /// </summary>
    protected FleeOnDamageData FleeOnDamageData { get; init; } = new(0.8f);

    /// <summary>
    /// Contains data pertaining to the <see cref="T:Scareable" /> component. This component is what enables small fish to swim away from the player and potential predators. Not assigned by default.
    /// </summary>
    protected ScareableData ScareableData { get; init; } = null;

    /// <summary>
    /// Contains data pertaining to creating the <see cref="T:AvoidObstacles" /> CreatureAction. This component is used by most creatures (everything besides leviathans) to avoid objects and/or terrain. Not assigned by default.
    /// </summary>
    protected AvoidObstaclesData AvoidObstaclesData { get; init; } = null;

    /// <summary>
    /// Contains data pertaining to creating the <see cref="T:AvoidTerrain" /> CreatureAction. This is a more advanced and expensive collision avoidance system used by leviathans.
    /// </summary>
    protected AvoidTerrainData AvoidTerrainData { get; init; } = null;

    /// <summary>
    /// Contains data pertaining to the <see cref="T:CreatureDeath" /> component.
    /// </summary>
    protected RespawnData RespawnData { get; init; } = new(false);

#if BELOWZERO
    /// <summary>
    /// Contains data pertaining to the <see cref="T:AggressiveToPilotingVehicle" /> component, which encourages creatures to target any small vehicle that the player may be piloting
    /// (this includes ANY vehicle that inherits from the <see cref="T:Vehicle" /> component i.e. the Seamoth or Prawn Suit). Not many creatures use this component, but ones that do
    /// will be VERY aggressive (Boneshark levels of aggression!).
    /// </summary>
    protected AggressiveToPilotingVehicleData AggressiveToPilotingVehicleData { get; init; } = null;
#endif

    /// <summary>
    /// A list of all data pertaining to the <see cref="T:AggressiveWhenSeeTarget" /> component, which enables the creature to become aggressive towards specific fauna/the player.
    /// </summary>
    protected List<AggressiveWhenSeeTargetData> AggressiveWhenSeeTargetList { get; init; }

    /// <summary>
    /// Contains data pertaining to creating the <see cref="T:AttackLastTarget" /> CreatureAction. Not assigned by default.
    /// </summary>
    protected AttackLastTargetData AttackLastTargetData { get; init; } = null;

    /// <summary>
    /// Contains data pertaining to creating the <see cref="T:AttackCyclops" /> CreatureAction. Not assigned by default.
    /// </summary>
    protected AttackCyclopsData AttackCyclopsData { get; init; } = null;

    /// <summary>
    /// <para>Contains data pertaining to adding the <see cref="T:SwimInSchool" /> CreatureAction.</para>
    /// <para>Each schooling creature chooses a single "leader" larger than itself (and of the same TechType) to follow. Therefore, the <see cref="P:ECCLibrary.Data.CreatureTemplate.SizeDistribution" /> property should be defined for this action to function properly.</para>
    /// <para>Not assigned by default.</para>
    /// </summary>
    protected SwimInSchoolData SwimInSchoolData { get; init; } = null;

    /// <summary>
    /// Mass in kg. Ranges from about 1.8f to 4050f. Default is 15kg.
    /// </summary>
    protected float Mass { get; init; } = 10f;

    /// <summary>
    /// Determines the distance for which certain calculations (such as Trail Managers) perform (or don't). It is recommended to increase these values for large creatures.
    /// </summary>
    protected BehaviourLODData BehaviourLODData { get; init; } = new();

    /// <summary>
    /// The FOV is used for detecting things such as prey. SHOULD BE NEGATIVE! This value has an expected range of [-1, 0]. Is 0f by default. A value of -1 means a given object is ALWAYS in view.
    /// </summary>
    protected float EyeFOV { get; init; } = 0.0f;

    /// <summary>
    /// Settings that determine basic attributes of the creature.
    /// </summary>
    protected CreatureTraitsData TraitsData { get; init; } = new(0.1f);

    /// <summary>
    /// If set to true, the Scanner Room can scan for this creature. False by default.
    /// </summary>
    protected bool ScannerRoomScannable { get; init; } = false;

    /// <summary>
    /// Possible sizes for this creature. Randomly picks a value in the range of 0 to 1. This value can not go above 1. Flat curve at 1 by default.
    /// </summary>
    protected AnimationCurve SizeDistribution { get; init; } = new(new Keyframe(0.0f, 1f), new Keyframe(1f, 1f));

    /// <summary>
    /// Goes hand in hand with the EcoTargetType. Please note the Player is a SHARK! Determines very few creature behaviours/interactions.
    /// </summary>
    protected BehaviourType BehaviourType { get; init; }

    /// <summary>
    /// Goes hand in hand with the BehaviourType. Determines many interactions with creatures, specifically how this creature is "located" or "targeted" by other creatures.
    /// </summary>
    protected EcoTargetType EcoTargetType { get; init; }

    /// <summary>
    /// Settings for growth in Alien Containment. Not assigned by default.
    /// </summary>
    protected WaterParkCreatureDataStruct? WaterParkCreatureData { get; init; }

    #endregion

    protected CustomCreaturePrefab(ModItem modItem, GameObject creaturePrefab) : base(modItem)
    {
        ModItem = modItem;
        this.creaturePrefab = creaturePrefab;
    }

    public override CreatureTemplate CreateTemplate()
    {
        CreatureTemplate template = new(creaturePrefab, BehaviourType, EcoTargetType, MaxHealth)
        {
            CellLevel = CellLevel,
            RespawnData = RespawnData,
            AttackCyclopsData = AttackCyclopsData,
            Mass = Mass,
            LocomotionData = LocomotionData,
            PhysicMaterial = PhysicMaterial,
            ScareableData = ScareableData,
            AcidImmune = true,
            SizeDistribution = SizeDistribution,
            TraitsData = TraitsData,
            AvoidTerrainData = AvoidTerrainData,
            AvoidObstaclesData = AvoidObstaclesData,
            ScannerRoomScannable = ScannerRoomScannable,
            SwimBehaviourData = SwimBehaviourData,
            SwimRandomData = SwimRandomData,
            AnimateByVelocityData = AnimateByVelocityData,
            AttackLastTargetData = AttackLastTargetData,
#if BELOWZERO
            AggressiveToPilotingVehicleData = AggressiveToPilotingVehicleData,
#else
            CanBeInfected = false,
#endif
            AggressiveWhenSeeTargetList = AggressiveWhenSeeTargetList,
            EyeFOV = EyeFOV,
            BehaviourLODData = BehaviourLODData,
            SwimInSchoolData = SwimInSchoolData,
            StayAtLeashData = StayAtLeashData,
            FleeWhenScaredData = FleeWhenScaredData,
            FleeOnDamageData = FleeOnDamageData,
        };
        if (WaterParkCreatureData != null) template.SetWaterParkCreatureData(WaterParkCreatureData.Value);
        template.SetCreatureComponentType<TCreature>();

        return template;
    }

    protected new virtual void Register() => base.Register();
    void ICustomCreaturePrefab.Register()
    {
        // Massive ECC L bruh
        AccessTools.PropertySetter(typeof(CreatureAsset), nameof(Template)).Invoke(this, new object[] { CreateTemplate() });
        Register();
    }

    public override void ApplyMaterials(GameObject gameObject) => MaterialHelpers.ApplySNShadersIncludingRemaps(gameObject, 1);
}

public interface ICustomCreaturePrefab
{
    ModItem ModItem { get; }

    void Register();
}
