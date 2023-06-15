using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    public sealed class ObjectInspectorDrawer
    {
        public Editor Editor => m_CachedEditor;
        public GUILayoutOption[] RectLayoutOptions { get; set; }
        public bool IsScrollable { get; set; } = true;
        public bool ShowInspector { get; set; } = true;

        private Editor m_CachedEditor;
        private Vector2 m_ScrollPos;


        public ObjectInspectorDrawer(Object obj, params GUILayoutOption[] options)
        {
            SetObject(obj);
            RectLayoutOptions = options;
        }

        public void DrawGUI()
        {
            m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos, RectLayoutOptions);

            if (m_CachedEditor != null && m_CachedEditor.serializedObject != null)
                m_CachedEditor.OnInspectorGUI();

            GUILayout.EndScrollView();
        }

        public void SetObject(Object obj)
        {
            if (obj == null)
                m_CachedEditor = null;
            else
                Editor.CreateCachedEditor(obj, null, ref m_CachedEditor);
        }
    }
}
