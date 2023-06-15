using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Toolbox.Editor;

namespace IdenticalStudios.ProceduralMotion
{
    [CustomEditor(typeof(MotionDataHandler))]
    public class MotionDataHandlerEditor : ToolboxEditor
    {
        private string[] m_MotionTypeNames;
        private int m_Selected;
        private bool m_IsVisualizing;
        private MotionDataHandler m_DataHandler;
        
        
        protected override void DrawCustomInspector()
        {
            base.DrawCustomInspector();

            if (!Application.isPlaying)
                return;
            
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox("Select a state and press on the ''visualize'' button to overwrite the current state of the motions.", MessageType.Info);
            
            using (new GUILayout.HorizontalScope())
            {
                m_Selected = EditorGUILayout.Popup(GUIContent.none, m_Selected, m_MotionTypeNames);

                GUI.backgroundColor = CustomGUIStyles.BlueColor;
                bool wasVisualizing = m_IsVisualizing;
                m_IsVisualizing = GUILayout.Toggle(m_IsVisualizing, "Visualize", "Button");
                GUI.backgroundColor = Color.white;

                if (wasVisualizing == m_IsVisualizing)
                    return;
                
                if (m_IsVisualizing)
                {
                    if (Enum.TryParse<MovementStateType>(m_MotionTypeNames[m_Selected], true, out var state))
                        m_DataHandler.Visualize(state);
                }
                else
                    m_DataHandler.Visualize(null);
            }
        }

        private void OnEnable()
        {
            m_DataHandler = (MotionDataHandler)target;
            m_MotionTypeNames = Enum.GetNames(typeof(MovementStateType)).ToArray();
        }
    }
}
