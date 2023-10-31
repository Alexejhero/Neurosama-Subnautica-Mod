using System.Linq;
using UnityEngine;

namespace SCHIZO.Options;

public sealed class RealtimeOptionUpdater : MonoBehaviour
{
    public ModOption modOption;

    public void OnEnable()
    {
        if (!modOption) return;

        if (modOption.disableIfAnyFalse.Any(o => !o.Value) || modOption.disableIfAnyTrue.Any(o => o.Value)) gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }
}
