using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCHIZO.Unity
{
    [CreateAssetMenu(fileName = "LoadingBackgroundCollection", menuName = "SCHIZO/Loading Background Collection")]
    public sealed class LoadingBackgroundCollection : ScriptableObject
    {
        public LoadingBackground[] backgrounds;
    }
}
