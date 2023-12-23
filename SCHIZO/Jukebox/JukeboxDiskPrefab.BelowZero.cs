using System.Collections.Generic;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using SCHIZO.Creatures.Components;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Jukebox;

public sealed class JukeboxDiskPrefab
{
    // prefab path "Misc/JukeboxDisk8.prefab"
    private const string DISK_CLASSID = "5108080f-242b-49e8-9b91-d01d6bbe138c";

    internal static Dictionary<string, JukeboxDiskPrefab> Prefabs = [];

    public CustomPrefab NautilusPrefab { get; }
    private JukeboxDiskPrefab(CustomJukeboxTrack track)
    {
        string prefabName = $"{nameof(CustomJukeboxTrack)}_{track.identifier}";

        NautilusPrefab = new CustomPrefab(prefabName, null, null);

        NautilusPrefab.SetSpawns(new SpawnLocation(track.diskSpawnLocation.position, track.diskSpawnLocation.rotation));

        NautilusPrefab.SetGameObject(new CloneTemplate(NautilusPrefab.Info, DISK_CLASSID)
        {
            ModifyPrefab = prefab =>
            {
                prefab.SetActive(false);
                if (track.diskPrefab)
                {
                    Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
                    renderers.ForEach(r => r.gameObject.SetActive(false));

                    GameObject customModel = GameObject.Instantiate(track.diskPrefab, prefab.transform, false);
                    customModel.transform.localPosition = Vector3.zero;

                    MaterialUtils.ApplySNShaders(customModel, 1);
                }

                JukeboxDisk diskComp = prefab.EnsureComponent<JukeboxDisk>();
                diskComp.track = track;
                if (!string.IsNullOrEmpty(track.unlockFmodEvent))
                    diskComp.acquireSound = AudioUtils.GetFmodAsset(track.unlockFmodEvent, FMODHelpers.GetId(track.unlockFmodEvent));
            }
        });
        NautilusPrefab.Register();
    }

    public static void Register(CustomJukeboxTrack track)
    {
        if (Prefabs.ContainsKey(track.identifier))
        {
            LOGGER.LogWarning($"Track {track.identifier} already registered, skipping prefab registration");
            return;
        }
        Prefabs[track.identifier] = new JukeboxDiskPrefab(track);
    }
}
