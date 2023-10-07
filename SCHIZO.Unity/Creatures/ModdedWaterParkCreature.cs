namespace SCHIZO.Unity.Creatures;

[DisallowMultipleComponent]
public sealed class ModdedWaterParkCreature : MonoBehaviour
{
    public float initialSize = 0.1f;
    public float maxSize = 0.6f;
    public float outsideSize = 1;
    public float daysToGrow = 1;
    public bool isPickupableOutside = true;
    public bool canBreed = true;
    public GameObject eggOrChildPrefab;
    public GameObject adultPrefab;
}
