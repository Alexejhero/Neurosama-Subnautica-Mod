using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace SCHIZO.Helpers;
internal static class CoroutineHelpers
{
    public static IEnumerator PoorMansAwait(this Task task, float checkInterval = 0)
    {
        if (task.Status == TaskStatus.Created)
            task.Start();

        object wait = checkInterval > 0
            ? new WaitForSeconds(checkInterval)
            : null;

        while (!task.IsCompleted)
        {
            yield return wait;
        }
    }
}
