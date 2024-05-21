using TriInspector;
using UnityEngine;

public class PingInstance : MonoBehaviour
{
    public PingType pingType;

	[Required]
	public Transform origin;
	public bool displayPingInManager = true;
	public float minDist = 15f;
	public float range = 10f;
	[Space]
	public bool visitable;
	public float visitDistance = 10f;
	public float visitDuration = 1f;

    private void OnEnable() {}
}
