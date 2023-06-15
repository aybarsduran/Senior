using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    public abstract class GroupDefinitionEditor : ToolboxEditor
    {
        protected override bool DrawScriptProperty => false;

        private GUIStyle BoxStyle => new GUIStyle("Box");
        private DataDefinitionBase m_DataDef;


        protected override void DrawCustomInspector()
        {
            serializedObject.Update();

            using (new GUILayout.VerticalScope(BoxStyle))
                DrawCustomPropertySkipIgnore("m_GroupName");

            using (new GUILayout.VerticalScope(BoxStyle))
            {
                GUI.enabled = false;
                DrawCustomPropertySkipIgnore("m_Id");
                if (ToolbarEditorWindowBase.HasActiveWindows)
                    EditorGUILayout.ObjectField("Asset", m_DataDef, target.GetType(), m_DataDef);
                GUI.enabled = true;
            }

            serializedObject.ApplyModifiedProperties();

            using (new GUILayout.VerticalScope(BoxStyle))
            {
                serializedObject.Update();
                if (!ToolbarEditorWindowBase.HasActiveWindows)
                    DrawCustomPropertySkipIgnore("m_Members");
                serializedObject.ApplyModifiedProperties();

                base.DrawCustomInspector();
            }
        }

        protected virtual void OnEnable()
        {
            m_DataDef = target as DataDefinitionBase;

            IgnoreProperty("m_Id");
            IgnoreProperty("m_Members");
            IgnoreProperty("m_GroupName");
            IgnoreProperty("m_MembersFolder");
        }
    }
}
