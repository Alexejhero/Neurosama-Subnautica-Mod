using SCHIZO.Utilities;
using TriInspector;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class UniqueIdentifier : MonoBehaviour
{
    [ReadOnly]
    public string _classId = "<assigned at runtime> (string)";
}
