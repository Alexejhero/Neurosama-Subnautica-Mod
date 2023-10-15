using NaughtyAttributes;
using SCHIZO.Attributes;
using SCHIZO.Utilities;
using UnityEngine;

public class AggressiveToPilotingVehicle : MonoBehaviour
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public LastTarget lastTarget;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required, ValidateType("Creature")] public MonoBehaviour creature;
    public float range = 20;
    public float aggressionPerSecond = 0.5f;
    public float targetPriority = 1;
}
