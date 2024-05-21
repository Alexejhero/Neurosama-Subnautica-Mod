using SCHIZO.Attributes;
using TriInspector;
using UnityEngine;

public class SignalPing : MonoBehaviour
{
    [Required, ExposedType("PingInstance")]
	public MonoBehaviour pingInstance;

    [ExposedType("PDANotification"), LabelText("VO")]
	public MonoBehaviour vo;

	public bool disableOnEnter;
}
