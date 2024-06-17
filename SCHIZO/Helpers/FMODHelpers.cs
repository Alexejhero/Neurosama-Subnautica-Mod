using System;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Nautilus.Utility;
using SCHIZO.Resources;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SCHIZO.Helpers;
internal static class FMODHelpers
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
    // TODO this can fail if the path/id aren't found but i cba to write the Try___ methods right now
    public static string GetPath(string guid)
    {
        if (!Guid.TryParse(guid, out Guid guid_)) return null;

        RuntimeManager.StudioSystem.lookupPath(guid_, out string pathFromId).CheckResult();
        return pathFromId;
    }
    public static string GetId(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        RuntimeManager.StudioSystem.lookupID(path, out Guid guidFromPath).CheckResult();
        return guidFromPath == default ? null : guidFromPath.ToString();
    }

    public static void PlayId(this FMOD_CustomEmitter emitter, string id, bool is3d = true)
        => PlayFmod(emitter, GetPath(id), id, is3d);
    public static void PlayPath(this FMOD_CustomEmitter emitter, string path, bool is3d = true)
        => PlayFmod(emitter, path, GetId(path), is3d);

    public static void PlayFmod(this FMOD_CustomEmitter emitter, string path, string id, bool is3d, float delay = 0)
    {
        if (string.IsNullOrEmpty(path)) return;

        if (!emitter)
        {
            PlayPath2D(path);
            return;
        }

        emitter.SetAsset(AudioUtils.GetFmodAsset(path, id));
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
}
