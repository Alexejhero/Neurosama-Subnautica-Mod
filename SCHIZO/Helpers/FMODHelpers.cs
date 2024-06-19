using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Nautilus.Utility;
using SCHIZO.Resources;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SCHIZO.Helpers;
internal static partial class FMODHelpers
{
    /// <summary>
    /// Attempts to load an FMOD bank from embedded resources.
    /// </summary>
    /// <param name="fileName">The filename of the bank - e.g. "Mod.bank".</param>
    /// <param name="throwOnFail">Throw a <see cref="BankLoadException"/> if the load did not succeed.</param>
    /// <exception cref="BankLoadException">Thrown with the result of the attempted load if <paramref name="throwOnFail"/> is set.</exception>
    public static void LoadBankFromResources(string fileName, bool throwOnFail = false)
    {
        RuntimeManager fmodRuntime = RuntimeManager.Instance;
        if (fmodRuntime.loadedBanks.TryGetValue(fileName, out RuntimeManager.LoadedBank alreadyLoaded))
        {
            alreadyLoaded.RefCount++;
            fmodRuntime.loadedBanks[fileName] = alreadyLoaded;
            return;
        }

        byte[] fmodBank = ResourceManager.GetEmbeddedBytes(fileName, throwOnFail);
        if (fmodBank == null) return; // already thrown above if throwIfMissing is set

        RESULT res = fmodRuntime.studioSystem.loadBankMemory(fmodBank, LOAD_BANK_FLAGS.NORMAL, out Bank bank);
        res.CheckResult();

        if (res != RESULT.OK || !bank.hasHandle())
        {
            if (throwOnFail)
                throw new BankLoadException(fileName, res);
        }
        else
        {
            fmodRuntime.loadedBanks[fileName] = new() { Bank = bank, RefCount = 1 };
        }
    }

    public static void LoadMasterBank(string name, bool throwOnFail = false)
    {
        LoadBankFromResources($"{name}.bank", throwOnFail);
        LoadBankFromResources($"{name}.strings.bank", throwOnFail);
    }

    public static void LoadSubBank(string name, bool throwOnFail = false)
    {
        LoadBankFromResources($"{name}.bank", throwOnFail);
    }

    // TODO these can fail if the path/id aren't found but i cba to write the Try___ methods right now
    public static string GetPath(string guid)
    {
        if (!Guid.TryParse(guid, out Guid guid_)) return null;

        return GetPath(guid_);
    }
    public static string GetPath(Guid guid)
    {
        RuntimeManager.StudioSystem.lookupPath(guid, out string pathFromId).CheckResult();
        return pathFromId;
    }
    public static string GetId(string path)
    {
        Guid guid = GetGuid(path);
        return guid == default ? null : guid.ToString();
    }
    public static Guid GetGuid(string path)
    {
        if (string.IsNullOrEmpty(path)) return default;

        RuntimeManager.StudioSystem.lookupID(path, out Guid guid).CheckResult();
        return guid;
    }

    public static void PlayGuid(this FMOD_CustomEmitter emitter, Guid guid, bool is3d = true)
        => PlayFmod(emitter, GetPath(guid), guid, is3d);
    public static void PlayPath(this FMOD_CustomEmitter emitter, string path, bool is3d = true)
        => PlayFmod(emitter, path, GetGuid(path), is3d);

    public static void PlayFmod(this FMOD_CustomEmitter emitter, string path, Guid guid, bool is3d, float delay = 0)
    {
        if (string.IsNullOrEmpty(path)) return;

        if (!emitter)
        {
            PlayPath2D(path);
            return;
        }

        emitter.SetAsset(GetFmodAsset(path, guid));
        emitter.SetParameterValue("3D", is3d ? 1 : 0);
        emitter.SetParameterValue("Delay", delay);

        emitter.Play();
    }

    public static EventInstance PlayPath2D(string path, float delay = 0)
    {
        if (string.IsNullOrEmpty(path)) return default;

        EventInstance evt = RuntimeManager.CreateInstance(path);
        evt.setParameterByName("3D", 0);
        evt.setParameterByName("Delay", delay);

        evt.start();
        return evt;
    }

    public static EventInstance PlayPath3DAttached(string path, Transform transform, float delay = 0)
    {
        if (string.IsNullOrEmpty(path)) return default;

        EventInstance evt = RuntimeManager.CreateInstance(path);
        evt.setParameterByName("3D", 1);
        evt.setParameterByName("Delay", delay);

        evt.set3DAttributes(transform.To3DAttributes());
        RuntimeManager.AttachInstanceToGameObject(evt, transform);
        evt.start();
        return evt;
    }

    public static void StopAllInstances(string path, bool stopImmediately = false)
    {
        if (string.IsNullOrEmpty(path)) return;

        RuntimeManager.StudioSystem.getEvent(path, out EventDescription _event);
        _event.getInstanceList(out EventInstance[] _instances);
        _instances?.ForEach(ev => ev.stop(stopImmediately ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT));
    }

    public static void StopAndRelease(this EventInstance evt)
    {
        evt.stop(STOP_MODE.IMMEDIATE);
        evt.release();
    }

    public static EventInstance PlayOneShot(string soundEvent, Vector3? position = null)
        => PlayOneShot(RuntimeManager.PathToGUID(soundEvent), position);

    public static EventInstance PlayOneShot(Guid guid, Vector3? position = null)
    {
        EventInstance evt = RuntimeManager.CreateInstance(guid);
        if (position.HasValue)
            evt.set3DAttributes(position.Value.To3DAttributes());
        evt.start();
        evt.release();
        return evt;
    }

    public static EventInstance PlayOneShotAttached(string soundEvent, GameObject gameObject)
        => PlayOneShotAttached(RuntimeManager.PathToGUID(soundEvent), gameObject);

    public static EventInstance PlayOneShotAttached(Guid guid, GameObject gameObject)
    {
        EventInstance evt = RuntimeManager.CreateInstance(guid);
        RuntimeManager.AttachInstanceToGameObject(evt, gameObject.transform, gameObject.GetComponent<Rigidbody>());
        evt.start();
        evt.release();
        return evt;
    }

    private static Dictionary<Guid, FMODAsset> _fmodAssetPool = [];
    public static FMODAsset GetFmodAsset(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        Guid guid = GetGuid(path);
        if (guid == default)
        {
            LOGGER.LogWarning($"Tried to get FMODAsset for unknown path \"{path}\"");
            return null;
        }
        return GetFmodAssetNoCheck(path, guid);
    }
    public static FMODAsset GetFmodAsset(Guid guid)
    {
        if (guid == default) return null;
        string path = GetPath(guid);

        if (path is null)
        {
            LOGGER.LogWarning($"Tried to get FMODAsset for unknown guid {guid}");
            return null;
        }

        return GetFmodAssetNoCheck(path, guid);
    }
    public static FMODAsset GetFmodAsset(string path, Guid guid)
    {
        if (string.IsNullOrEmpty(path)) return null;

        if (guid != GetGuid(path))
        {
            LOGGER.LogWarning($"Tried to get FMODAsset for path \"{path}\" and guid {guid} which don't match!");
            return null;
        }
        return GetFmodAssetNoCheck(path, guid);
    }

    private static FMODAsset GetFmodAssetNoCheck(string path, Guid guid)
    {
        if (_fmodAssetPool.TryGetValue(guid, out FMODAsset asset) && asset)
            return asset;

        asset = ScriptableObject.CreateInstance<FMODAsset>();
        asset.path = path;
        asset.id = guid.ToString();
        _fmodAssetPool[guid] = asset;
        return asset;
    }
}
