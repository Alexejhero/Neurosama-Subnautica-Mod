using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using FMOD;
using SCHIZO.Helpers;
using SCHIZO.Extensions;
using SCHIZO.Creatures.Ermfish;
using Nautilus.Handlers;
using System.IO;
using System;
using FMODUnity;
using Nautilus.Utility;

namespace SCHIZO.Events.ErmMoon;

public class ErmRaptureEvent : CustomEvent
{
    public override bool IsOccurring => _isOccurring;
    private bool _isOccurring;

    public float EventDurationSeconds = 60f;
    public float SearchRange = 250f;

    private ErmMoonEvent _moonEvent;
    private GameObject _moon;

    private float _eventStartTime;

    private HashSet<Creature> _seenErmfish;
    private const float _minSearchInterval = 1f;
    private float _lastSearchTime;

    private FMOD_CustomEmitter _ermCallEmitter;
    private float _ermCallDuration;
    private float _ermCallPlaybackPosition; // where we are in the sound file
    private float _ermCallCooldown = 5f;
    private float _lastErmCallTime = -999f;
    private float maxVolume = 1.5f;
    private float minAudibleDepth = -250f;
    private REVERB_PROPERTIES _savedReverb2;
    private REVERB_PROPERTIES _savedReverb3;
    private REVERB_PROPERTIES _openAirReverb;
    private REVERB_PROPERTIES _underwaterReverb;
    private bool _happenedTonight;

    private uSkyManager _skyManager;
    private float _normalCloudBrightness;
    private float _eventCloudBrightness;

    private void Awake()
    {
        _moonEvent = CustomEventManager.main.GetEvent<ErmMoonEvent>();
        _skyManager = FindObjectOfType<uSkyManager>();
        _seenErmfish = new();
        _moon = new GameObject();
        _moon.transform.eulerAngles = new Vector3(315, 0, 0);
        _moon.transform.position = gameObject.transform.position + _moon.transform.forward * 10000f;

        _ermCallEmitter = _moon.AddComponent<FMOD_CustomEmitter>();
        string ermCallGuid = Guid.NewGuid().ToString();
        // don't look
        string bus = "bus:/master/SFX_for_pause/PDA_pause/all/Sounds_muted by pain";
        Sound ermCallSound = CustomSoundHandler.RegisterCustomSound(ermCallGuid,
                Path.Combine(AssetLoader.AssetsFolder, "sounds", "events", "erm_call_sky.mp3"),
                bus,
                MODE._3D_LINEARROLLOFF);
        RuntimeManager.GetBus(bus).unlockChannelGroup();

        ermCallSound.set3DMinMaxDistance(1000, 30000);
        _ermCallEmitter.SetAsset(AudioUtils.GetFmodAsset(ermCallGuid, "erm_call_sky"));
        _ermCallEmitter.followParent = true;

        RuntimeManager.CoreSystem.getReverbProperties(2, out _savedReverb2);
        RuntimeManager.CoreSystem.getReverbProperties(3, out _savedReverb3);
        _openAirReverb = PRESET.PLAIN();
        _underwaterReverb = PRESET.UNDERWATER();

        ermCallSound.getLength(out uint ermCallDurationMillis, TIMEUNIT.MS);
        _ermCallDuration = ermCallDurationMillis / 1000f;

        _normalCloudBrightness = _skyManager.cloudNightBrightness;
        _eventCloudBrightness = _normalCloudBrightness * 20;
    }

    protected override bool ShouldStartEvent()
    {
        if (!_moonEvent.IsOccurring)
        {
            _happenedTonight = false;
            return false;
        }
        
        return !_happenedTonight
            && gameObject.transform.position.y > minAudibleDepth
            && DayNightHelpers.isNight;
    }

    protected override void UpdateLogic()
    {
        float time = Time.fixedTime;
        if (time > _eventStartTime + EventDurationSeconds || !_moonEvent.IsOccurring)
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
        float timeProp = (time - _eventStartTime) / EventDurationSeconds;

        // poor man's animator curve
        if (timeProp is < 0.05f)
            _skyManager.cloudNightBrightness = Mathf.Lerp(_normalCloudBrightness, _eventCloudBrightness, timeProp * 20);
        else if (timeProp is > 0.95f)
            _skyManager.cloudNightBrightness = Mathf.Lerp(_normalCloudBrightness, _eventCloudBrightness, (1 - timeProp) * 20);
    }

    private void UpdateErmCall(float time, float deltaTime)
    {
        if (!IsOccurring) return;

        if (_ermCallEmitter.playing)
        {
            _ermCallPlaybackPosition += deltaTime;
            if (_ermCallPlaybackPosition > _ermCallDuration)
            {
                _ermCallPlaybackPosition = 0;
                _ermCallEmitter.Stop();
                RuntimeManager.CoreSystem.setReverbProperties(2, ref _savedReverb2);
                RuntimeManager.CoreSystem.setReverbProperties(3, ref _savedReverb3);
                return;
            }
        }
        else
        {
            bool onCooldown = _lastErmCallTime + _ermCallDuration + _ermCallCooldown > time;
            bool eventAlmostOver = 2 * _ermCallDuration > _eventStartTime + EventDurationSeconds - time;
            if (onCooldown || eventAlmostOver) return;
            _lastErmCallTime = time;
            RuntimeManager.CoreSystem.setReverbProperties(2, ref _openAirReverb);
            RuntimeManager.CoreSystem.setReverbProperties(3, ref _underwaterReverb);
            _ermCallEmitter.Play();
        }

        CustomSoundHandler.TryGetCustomSoundChannel(_ermCallEmitter.GetInstanceID(), out Channel channel);

        float posY = gameObject.transform.position.y;
        float volumeDepthFrac = posY >= 0 ? 0 // above water - full volume
            : 0.5f + 0.5f * (posY / minAudibleDepth); // underwater - base 50%, fade to 0 with more depth
        volumeDepthFrac = volumeDepthFrac.Clamp01();

        float volume = maxVolume * (1 - volumeDepthFrac);

        //LOGGER.LogWarning($"UpdateErmCall volume {volume} (at depth {posY}/{volumeDepthFrac})");
        channel.setVolume(volume);
        channel.setReverbProperties(2, posY >= 0 ? 1 : 0);
        channel.setReverbProperties(3, posY < 0 ? 1 : 0);
        _ermCallEmitter.UpdateEventAttributes();
    }

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
                > 1225 => 80f, // 35m
                > 625 => 40f, // 25m
                _ => 20f
            };

            swim.SwimTo(_moon.transform.position, swimVelocity);
        }
    }

    public override void StartEvent()
    {
        if (!DayNightHelpers.isNight)
            DevConsole.SendConsoleCommand("night");
        if (!_moonEvent.IsOccurring)
            _moonEvent.StartEvent();

        foreach (Creature ermfish in GetErmfishInRange())
            ReceiveErmSignal(ermfish);
        _lastSearchTime = Time.time;

        LOGGER.LogWarning("An Erm rapture has begun.");
        _isOccurring = true;
        _happenedTonight = true;
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
        _seenErmfish.Clear();
        _skyManager.cloudNightBrightness = _normalCloudBrightness; // if ended manually
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
        //Light light = ermfish.gameObject.AddComponent<Light>();
        //light.intensity = 1f;
        //light.range = 20f;
    }

    private void EndErmSignal(Creature ermfish)
    {
        SwimBehaviour swim = ermfish.GetComponent<SwimBehaviour>();
        swim.Idle();
        swim.LookForward();
        ermfish.GetComponent<WorldForces>().handleGravity = true;
        //Destroy(ermfish.GetComponent<Light>());
    }

    private IEnumerable<Creature> GetErmfishInRange()
    {
        return PhysicsHelpers.ObjectsInRange(gameObject, SearchRange)
            .OfTechType(ErmfishLoader.ErmfishTechTypes)
            .SelectComponent<Creature>();
    }
}
