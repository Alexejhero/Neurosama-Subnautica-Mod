using UnityEngine;

namespace SCHIZO.Registering;

public interface IPrefabInit
{
    /// <summary>Called once per prefab when the prefab is registered.</summary>
    /// <remarks>
    /// Use this to set up "static" things - registered only once and persisting through the whole application session.
    /// </remarks>
    void PrefabInit(GameObject prefab);
}
