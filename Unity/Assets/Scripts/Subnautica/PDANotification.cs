using UnityEngine;

public class PDANotification : MonoBehaviour
{
	public string text;
	public FMODAsset sound;
	public float minTimeBetweenPlayback;

	public bool allowDuringConversation;
	[Tooltip("Replaces the Text/Sound during conversation.")]
	public FMODAsset soundDuringConversation;
}
