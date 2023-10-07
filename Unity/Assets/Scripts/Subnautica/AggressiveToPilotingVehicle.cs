using NaughtyAttributes;
using UnityEngine;

public class AggressiveToPilotingVehicle : MonoBehaviour
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public LastTarget lastTarget;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public Creature creature;
    public float range = 20;
    public float aggressionPerSecond = 0.5f;
    public float targetPriority = 1;
}
