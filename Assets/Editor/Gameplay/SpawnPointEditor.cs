using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpawnPoint))]
    public class SpawnPointEditor : ToolboxEditor
    {
        private SpawnPoint m_Target;


        protected override void DrawCustomInspector()
        {
            base.DrawCustomInspector();

            CustomGUILayout.Separator();

            if (GUILayout.Button("Snap to ground"))
                m_Target.SnapToGround();
        }

        private void OnEnable()
        {
            m_Target = (SpawnPoint)serializedObject.targetObject;
        }
    }
}