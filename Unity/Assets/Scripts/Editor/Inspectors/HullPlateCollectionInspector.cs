using System.Linq;
using NaughtyAttributes.Editor;
using PropertyDrawers;
using SCHIZO.Unity.HullPlates;
using UnityEditor;

namespace Inspectors
{
    [CustomEditor(typeof(HullPlateCollection))]
    public sealed class HullPlateCollectionInspector : NaughtyInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            HullPlateCollection collection = (HullPlateCollection) target;
            HullPlateDrawer.NormalHullPlates = collection.hullPlates.Select(h => h.GetInstanceID()).ToList();
            HullPlateDrawer.DeprecatedHullPlates = collection.deprecatedHullPlates.Select(h => h.GetInstanceID()).ToList();
        }

        private new void OnEnable()
        {
            base.OnEnable();

            HullPlateCollection collection = (HullPlateCollection) target;
            HullPlateDrawer.NormalHullPlates = collection.hullPlates.Select(h => h.GetInstanceID()).ToList();
            HullPlateDrawer.DeprecatedHullPlates = collection.deprecatedHullPlates.Select(h => h.GetInstanceID()).ToList();
        }

        private new void OnDisable()
        {
            base.OnDisable();

            HullPlateDrawer.NormalHullPlates.Clear();
            HullPlateDrawer.DeprecatedHullPlates.Clear();
        }
    }
}
