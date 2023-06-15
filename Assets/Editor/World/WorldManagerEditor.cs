using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.WorldManagement
{
    [CustomEditor(typeof(WorldManager))]
    public class WorldManagerEditor : ToolboxEditor
    {
        private WorldManager m_WorldManager;


        protected override void DrawCustomInspector()
        {
            base.DrawCustomInspector();

            CustomGUILayout.Separator();
            EditorGUILayout.Space();

            GUILayout.Label("Debugging", EditorStyles.boldLabel);

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Debug info will be shown at play-time!", MessageType.Info);
                return;
            }

            m_WorldManager.DisplayDebugInfo();

            GUI.changed = true;
        }

        private void OnEnable()
        {
            m_WorldManager = (WorldManager)serializedObject.targetObject;
        }
    }
}
