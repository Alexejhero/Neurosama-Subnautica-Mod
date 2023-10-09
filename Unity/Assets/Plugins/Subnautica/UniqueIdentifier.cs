using NaughtyAttributes;
using UnityEngine;

[DisallowMultipleComponent]
public abstract class UniqueIdentifier : MonoBehaviour
{
    [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public string classId;
}
