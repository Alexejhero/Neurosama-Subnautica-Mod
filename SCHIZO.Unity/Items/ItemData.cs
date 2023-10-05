using SCHIZO.Unity.Sounds;

namespace SCHIZO.Unity.Items;

[CreateAssetMenu(menuName = "SCHIZO/Items/Item Data")]
public sealed class ItemData : ScriptableObject
{
    public GameObject prefab;
    public Sprite icon;
    public BaseSoundCollection sounds;
}
