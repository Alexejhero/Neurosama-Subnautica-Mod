using SCHIZO.Sounds.Collections;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items.Data
{
    [CreateAssetMenu(menuName = "SCHIZO/Items/PDA Encyclopedia Info")]
    [DeclareBoxGroup("Scanning")]
    [DeclareBoxGroup("Databank")]
    public sealed class PDAEncyclopediaInfo : ScriptableObject
    {
        [Group("Scanning")] public float scanTime = 3;
        [Group("Scanning")] public Sprite unlockSprite;
        [Group("Scanning")] public bool isImportantUnlock;
        [Group("Scanning")] public SoundCollectionInstance scanSounds;

        [Group("Databank")] public string encyPath;
        [Group("Databank")] public string title;
        [Group("Databank")] public Texture2D texture;
        [Group("Databank")] public TextAsset description;
    }
}
