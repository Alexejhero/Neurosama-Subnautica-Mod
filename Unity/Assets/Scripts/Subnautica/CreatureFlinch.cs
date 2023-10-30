using NaughtyAttributes;
using UnityEngine;

public class CreatureFlinch : MonoBehaviour
{
    [Required] public Animator animator;

    public float interval = 1;
    public float damageThreshold = 10;
}
