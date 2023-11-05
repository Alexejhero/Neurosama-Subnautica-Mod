using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

namespace SCHIZO.Helpers
{
    [Obsolete("", true)]
    public class BetterDropdownList<T> : Dictionary<string, T>, IDropdownList
    {
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            foreach (KeyValuePair<string, T> kv in this)
            {
                yield return new KeyValuePair<string, object>(kv.Key, kv.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
