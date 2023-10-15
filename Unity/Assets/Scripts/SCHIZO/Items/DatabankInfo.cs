using NaughtyAttributes;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Items
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/Databank Info")]
    public sealed class DatabankInfo : ScriptableObject
    {
        [BoxGroup("Scanning")] public float scanTime = 3;
        [BoxGroup("Scanning")] public Sprite unlockSprite;
        [BoxGroup("Scanning")] public bool isImportantUnlock;
        [BoxGroup("Scanning")] public BaseSoundCollection scanSounds;

        [BoxGroup("Databank")] public string encyPath;
        [BoxGroup("Databank")] public string title;
        [BoxGroup("Databank")] public Texture2D texture;
        [BoxGroup("Databank")] public TextAsset description;
    }
}
