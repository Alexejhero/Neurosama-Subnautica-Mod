using SCHIZO.TriInspector;
using TriInspector;
using UnityEngine;

public class CreatureFlinch : TriMonoBehaviour
{
    [ComponentReferencesGroup, Required] public Animator animator;

    public float interval = 1;
    public float damageThreshold = 10;
}
