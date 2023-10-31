using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Interop.NaughtyAttributes
{
    public abstract class NaughyMonoBehaviour : MonoBehaviour
    {
        private protected sealed class Required_string : ValidateInputAttribute
        {
            public Required_string(string message = null) : base(nameof(_required_string), message)
            {
            }
        }

        private bool _required_string(string val) => !string.IsNullOrWhiteSpace(val);
    }
}
