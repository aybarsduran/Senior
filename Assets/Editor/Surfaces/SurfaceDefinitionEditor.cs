using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.Surfaces
{
    [CustomEditor(typeof(SurfaceDefinition))]
    public class SurfaceDefinitionEditor : ToolboxEditor
    {
        protected override bool DrawScriptProperty => false;

        private SurfaceDefinition m_Surface;


        protected override void DrawCustomInspector()
        {
            if (m_Surface == null)
                m_Surface = target as SurfaceDefinition;

            if (SurfaceManagerWindow.IsActive)
            {
                GUI.enabled = false;
                GUILayout.BeginHorizontal("Box");
                EditorGUILayout.ObjectField(m_Surface, typeof(SurfaceDefinition), m_Surface);
                GUILayout.EndHorizontal();
                GUI.enabled = true;
            }

            GUILayout.Space(6f);

            base.DrawCustomInspector();
        }
    }
}
