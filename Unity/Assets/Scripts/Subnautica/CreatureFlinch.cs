using NaughtyAttributes;
using UnityEngine;

public class CreatureFlinch : MonoBehaviour
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public Animator animator;

    public float interval = 1;
    public float damageThreshold = 10;
}
