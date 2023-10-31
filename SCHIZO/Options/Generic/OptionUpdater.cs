using System.Linq;
using UnityEngine;

namespace SCHIZO.Options.Generic;

partial class OptionUpdater : MonoBehaviour
{
    public ModOption modOption;

    private void OnEnable()
    {
        if (!modOption) return;
        UpdateOption();
    }

    public virtual void UpdateOption()
    {
        if (modOption.disableIfAnyFalse.Any(o => !o.Value) || modOption.disableIfAnyTrue.Any(o => o.Value)) gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }
}
