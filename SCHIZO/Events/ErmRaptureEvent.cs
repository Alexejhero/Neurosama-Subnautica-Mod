using System.Collections.Generic;
using SCHIZO.Events.ErmCon;
using UnityEngine;
using System.Linq;
using FMOD;
using SCHIZO.Helpers;
using SCHIZO.Extensions;
using SCHIZO.Creatures.Ermfish;
using Nautilus.Handlers;
using System.IO;
using System;
using SCHIZO.Events.ErmMoon;
using FMODUnity;
using FMOD.Studio;

namespace SCHIZO.Events;

public class ErmRaptureEvent : CustomEvent
{
    public override bool IsOccurring => _isOccurring;
    private bool _isOccurring;

    public float EventDurationSeconds = 120f;
    public float EventDurationDayFraction => EventDurationSeconds / 1200f;
    public float SearchRange = 250f;

    private ErmConEvent _conEvent;
    private bool _conJustEnded;
    private ErmMoonEvent _moonEvent;
    private GameObject _moon;

    private float _eventStartTime;

    private HashSet<Creature> _seenErmfish;
    private const float _minSearchInterval = 1f;
    private float _lastSearchTime;

    private FMOD_CustomEmitter _underwaterEmitter;
    private FMOD_CustomEmitter _openAirEmitter;
    private FMODAsset _underwaterAsset;
    private FMODAsset _openAirAsset;
    private bool _isAboveWater;
    private float _ermCallDuration;
    private float _ermCallPlaybackPosition; // where we are in the sound file
    private float _ermCallCooldown = 5f;
    private float _lastErmCallTime = -999f;
    private Sound _underwaterSound;
    private Sound _openAirSound;

    private uSkyManager _skyManager;
    private float _normalCloudBrightness;
    private float _eventCloudBrightness;

    private void Awake()
    {
        _conEvent = CustomEventManager.main.GetEvent<ErmConEvent>();
        _moonEvent = CustomEventManager.main.GetEvent<ErmMoonEvent>();
        _skyManager = FindObjectOfType<uSkyManager>();
        _seenErmfish = new();
        _moon = new GameObject();
        _moon.transform.eulerAngles = new Vector3(315, 0, 0);
        _moon.transform.position = gameObject.transform.position + _moon.transform.forward * 10000f;

        _underwaterEmitter = _moon.AddComponent<FMOD_CustomEmitter>();
        _openAirEmitter = _moon.AddComponent<FMOD_CustomEmitter>();
        _underwaterEmitter.debug = _openAirEmitter.debug = true;
        _conEvent.Ended += delegate() { _conJustEnded = true; };
        string underwaterGuid = Guid.NewGuid().ToString();
        string openAirGuid = Guid.NewGuid().ToString();
        // don't look
        _underwaterSound = CustomSoundHandler.TryGetCustomSound(underwaterGuid, out Sound underSound)
            ? underSound
            : CustomSoundHandler.RegisterCustomSound(underwaterGuid,
                Path.Combine(AssetLoader.AssetsFolder, "sounds", "events", "erm_call_muted.mp3"),
                "bus:/master/SFX_for_pause/PDA_pause/all/SFX/backgrounds/underwater_backgrounds/jellyshroom_caves", MODE._3D_LINEARROLLOFF);
        _openAirSound = CustomSoundHandler.TryGetCustomSound(openAirGuid, out Sound overSound)
            ? overSound
            : CustomSoundHandler.RegisterCustomSound(openAirGuid,
                Path.Combine(AssetLoader.AssetsFolder, "sounds", "events", "erm_call_sky.mp3"),
                "bus:/master/SFX_for_pause/PDA_pause/all/SFX/dives", MODE._3D_LINEARROLLOFF);

        _underwaterSound.set3DMinMaxDistance(10000, 30000);
        _openAirSound.set3DMinMaxDistance(10000, 30000);
        
        _underwaterAsset = ScriptableObject.CreateInstance<FMODAsset>();
        _underwaterAsset.path = underwaterGuid;
        _underwaterAsset.id = "erm_call_underwater";
        _openAirAsset = ScriptableObject.CreateInstance<FMODAsset>();
        _openAirAsset.path = openAirGuid;
        _openAirAsset.id = "erm_call_open_air";

        _underwaterEmitter.SetAsset(_underwaterAsset);
        _openAirEmitter.SetAsset(_openAirAsset);

        // the two sounds MUST have the same duration to loop/switch properly
        _underwaterSound.getLength(out uint ermCallDurationMillis, TIMEUNIT.MS);
        _ermCallDuration = ermCallDurationMillis / 1000f;

        _normalCloudBrightness = _skyManager.cloudNightBrightness;
        _eventCloudBrightness = _normalCloudBrightness * 20;
    }

    protected override bool ShouldStartEvent()
    {
        if (!(_conJustEnded && _moonEvent.IsOccurring)) return false;
        bool willBeNightAtTheEnd = (DayNightUtils.dayScalar + EventDurationDayFraction) % 1 < 0.14f;
        return DayNightHelpers.isNight && willBeNightAtTheEnd;
    }

    protected override void UpdateLogic()
    {
        float time = Time.fixedTime;
        if (time > _eventStartTime + EventDurationSeconds)
        {
            // TODO clean up spawned ermfish
            EndEvent();
            return;
        }

        UpdateErmCall(time, Time.fixedDeltaTime);
        ConvertNewErmfish(time);
        // TODO spawn new ermfish around to make it feel more populated
    }

    protected override void UpdateRender()
    {
        float time = Time.time;
        if (time < _eventStartTime + 5) // poor man's lerp
        {
            var remaining = _eventCloudBrightness - _skyManager.cloudNightBrightness;
            _skyManager.cloudNightBrightness += remaining * 0.10f;
        }
        if (time + 5 > _eventStartTime + EventDurationSeconds)
        {
            var remaining = _skyManager.cloudNightBrightness - _normalCloudBrightness;
            _skyManager.cloudNightBrightness -= remaining * 0.10f;
        }
    }

    private void UpdateErmCall(float time, float deltaTime)
    {
        if (!IsOccurring) return;
        bool wasAboveWater = _isAboveWater;
        _isAboveWater = gameObject.transform.position.y > 0;

        // abandon hope beyond this point
        FMOD_CustomEmitter emitter = PickEmitter(_isAboveWater);
        FMOD_CustomEmitter otherEmitter = PickTheOtherEmitter(_isAboveWater);
        FMOD_CustomEmitter wasPlaying = PickEmitter(wasAboveWater);
        LOGGER.LogWarning($"EMITTER HANDLE: {emitter.evt.handle}");
        bool shouldUpdateEmitters = true;
        if (emitter.playing || otherEmitter.playing)
        {
            _ermCallPlaybackPosition += deltaTime;
            if (_ermCallPlaybackPosition > _ermCallDuration)
            {
                _ermCallPlaybackPosition = 0;
                emitter.Stop();
                otherEmitter.Stop();

                return;
            }
            else
            {
                // skip updating if it's the same emitter
                shouldUpdateEmitters = false;
                //shouldUpdateEmitters &= emitter != wasPlaying;
            }
        }
        else
        {
            bool onCooldown = _lastErmCallTime + _ermCallDuration + _ermCallCooldown > time;
            bool eventAlmostOver = 2 * _ermCallDuration > (_eventStartTime + EventDurationSeconds) - time;
            if (onCooldown || eventAlmostOver)
                shouldUpdateEmitters = false;
            else
            {
                _lastErmCallTime = time;
                emitter.Play();
                otherEmitter.Play();
            }
        }
        if (!shouldUpdateEmitters) return;

        // whatever fmod sucks
        var pos = (int) (_ermCallPlaybackPosition * 1000);
        emitter.CacheEventInstance();
        otherEmitter.CacheEventInstance();
        if (!emitter.evt.hasHandle())
            LOGGER.LogError($"FMOD: No handle on {WhichEmitter(emitter)}");
        if (!otherEmitter.evt.hasHandle())
            LOGGER.LogError($"FMOD: No handle on {WhichEmitter(otherEmitter)}");
        LOGGER.LogWarning($"{WhichEmitter(otherEmitter)} stop {otherEmitter.evt.setTimelinePosition(pos)}");
        LOGGER.LogWarning(otherEmitter.evt.getTimelinePosition(out var newPos));
        LOGGER.LogWarning($"{pos} {newPos}");
        otherEmitter.evt.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        emitter.evt.start();
        LOGGER.LogWarning($"{WhichEmitter(emitter)} start {emitter.evt.setTimelinePosition(pos)}");
        //LOGGER.LogWarning(emitter.evt.setVolume(1));
        //LOGGER.LogWarning(otherEmitter.evt.setVolume(0));
    }
    private FMOD_CustomEmitter PickEmitter(bool isAboveWater)
        => isAboveWater ? _openAirEmitter : _underwaterEmitter;
    private FMOD_CustomEmitter PickTheOtherEmitter(bool isAboveWater)
        => isAboveWater ? _underwaterEmitter : _openAirEmitter;

    private string WhichEmitter(FMOD_CustomEmitter emitter)
        => emitter == _underwaterEmitter ? "underwater" : "openair";

    private FMODAsset PickAsset(FMOD_CustomEmitter emitter)
        => emitter == _underwaterEmitter ? _underwaterAsset : _openAirAsset;

    private void ConvertNewErmfish(float time)
    {
        if (time < _lastSearchTime + _minSearchInterval)
            return;

        IEnumerable<Creature> fishPlural = GetErmfishInRange()
            .Where(fish => fish && !_seenErmfish.Contains(fish));
        _lastSearchTime = time;
        foreach (Creature fish in fishPlural)
        {
            if (!fish) continue; // literally impossible but i just know ermfish would find a way

            ReceiveErmSignal(fish);

            SwimBehaviour swim = fish.GetComponent<SwimBehaviour>();
            Vector3 playerPos = gameObject.transform.position;
            float distSqr = fish.transform.position.DistanceSqrXZ(playerPos);
            float swimVelocity = distSqr switch
            {
                >1225 => 40, // 35m
                >625 => 20, // 25m
                _ => 10
            };

            swim.SwimTo(_moon.transform.position, swimVelocity);
        }
    }

    public override void StartEvent()
    {
        // refactor later (deadline looming)
        bool isNight = DayNightUtils.dayScalar is > 0.87f or < 0.14f;
        if (!isNight)
            DevConsole.SendConsoleCommand("night");
        if (!_moonEvent.IsOccurring)
            _moonEvent.StartEvent();

        foreach (Creature ermfish in GetErmfishInRange())
            ReceiveErmSignal(ermfish);
        _lastSearchTime = Time.time;

        LOGGER.LogWarning("An Erm rapture has begun.");
        _isOccurring = true;
        _eventStartTime = Time.time;
        base.StartEvent();
    }

    public override void EndEvent()
    {
        foreach (Creature ermfish in _seenErmfish)
        {
            if (!ermfish) continue;
            EndErmSignal(ermfish);
        }
        _skyManager.cloudNightBrightness /= 20f;
        _seenErmfish.Clear();
        _isOccurring = false;
        base.EndEvent();
    }

    private void ReceiveErmSignal(Creature ermfish)
    {
        ermfish.GetComponent<WorldForces>().handleGravity = false;
        Locomotion loco = ermfish.GetComponent<Locomotion>();
        loco.canMoveAboveWater = true;
        loco.lookTarget = _moon.transform;
        _seenErmfish.Add(ermfish);
        Light light = ermfish.gameObject.AddComponent<Light>();
        light.intensity = 1f;
        light.range = 20f;
    }

    private void EndErmSignal(Creature ermfish)
    {
        SwimBehaviour swim = ermfish.GetComponent<SwimBehaviour>();
        swim.Idle();
        swim.LookForward();
        ermfish.GetComponent<WorldForces>().handleGravity = true;
        Destroy(ermfish.GetComponent<Light>());
    }

    private IEnumerable<Creature> GetErmfishInRange()
    {
        return PhysicsHelpers.ObjectsInRange(gameObject, SearchRange)
            .OfTechType(ErmfishLoader.ErmfishTechTypes)
            .SelectComponent<Creature>();
    }
}
