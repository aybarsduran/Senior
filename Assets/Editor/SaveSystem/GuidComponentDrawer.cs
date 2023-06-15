using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.SaveSystem
{
    [CustomEditor(typeof(GuidComponent))]
    public class GuidComponentDrawer : Editor
    {
        private GuidComponent guidComp;

        public override void OnInspectorGUI()
        {
            if (guidComp == null)
            {
                guidComp = (GuidComponent)target;
            }

            // Draw label
            EditorGUILayout.LabelField("Guid:", guidComp.GetGuid().ToString());
        }
    }
}