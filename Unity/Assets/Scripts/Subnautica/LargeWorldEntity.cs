using UnityEngine;

[DisallowMultipleComponent]
public class LargeWorldEntity : MonoBehaviour
{
    public CellLevel cellLevel = CellLevel.Medium;

    public enum CellLevel
    {
        Near = 0,
        Medium = 1,
        Far = 2,
        VeryFar = 3,
        Batch = 10, // 0x0000000A
        Global = 100, // 0x00000064
    }
}
