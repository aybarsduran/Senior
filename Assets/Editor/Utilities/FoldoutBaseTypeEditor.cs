using Toolbox.Editor;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace IdenticalStudios
{
    using Object = UnityEngine.Object;

    public abstract class FoldoutBaseTypeEditor<T> : ToolboxEditor where T : Object
    {
        private string m_BaseTypeName;

        private static bool m_BaseFoldout;
        private static bool m_ChildFoldout = true;

        private readonly List<SerializedProperty> m_IgnoredProperties = new List<SerializedProperty>();
        private readonly Color s_BoxColor = new Color(0.935f, 0.935f, 0.935f, 1f);


        protected override void DrawCustomInspector()
        {
            //try to draw the first property (m_Script)
            using (new EditorGUI.DisabledScope(true))
            {
                DrawCustomProperty("m_Script");
            }

            if (IsChildOfBaseType())
            {
                m_BaseFoldout = EditorGUILayout.Foldout(m_BaseFoldout, $"Settings ({m_BaseTypeName})", true, EditorStyles.foldoutHeader);
                EditorGUILayout.BeginVertical("Box");

                GUI.color = s_BoxColor;
                if (m_BaseFoldout)
                {
                    serializedObject.Update();

                    foreach (var prop in m_IgnoredProperties)
                        ToolboxEditorGui.DrawToolboxProperty(prop);

                    serializedObject.ApplyModifiedProperties();
                }
                GUI.color = Color.white;

                EditorGUILayout.EndVertical();

                if (!m_BaseFoldout)
                    GUILayout.Space(-8f);

                m_ChildFoldout = EditorGUILayout.Foldout(m_ChildFoldout, $"Settings ({target.GetType().Name.ToUnityLikeNameFormat()})", true, EditorStyles.foldoutHeader);
                EditorGUILayout.BeginVertical("Box");

                GUI.color = s_BoxColor;
                if (m_ChildFoldout)
                    base.DrawCustomInspector();
                GUI.color = Color.white;

                EditorGUILayout.EndVertical();

                GUILayout.Space(-8f);
            }
            else
                base.DrawCustomInspector();

        }

        protected virtual void OnEnable()
        {
            m_BaseTypeName = typeof(T).Name.ToUnityLikeNameFormat();

            if (IsChildOfBaseType())
            {
                var fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var field in fields)
                {
                    SerializedProperty property = serializedObject.FindProperty(field.Name);

                    if (property != null)
                    {
                        m_IgnoredProperties.Add(property);
                        IgnoreProperty(property);
                    }
                }
            }
        }

        private bool IsChildOfBaseType() => target.GetType() != typeof(T);
    }
}
