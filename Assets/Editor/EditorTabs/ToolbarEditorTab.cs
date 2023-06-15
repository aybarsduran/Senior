using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    public abstract class ToolbarEditorTab
    {
        public string TabName => m_TabName;
        public Rect Rect => m_Window.position;

        protected readonly float DefaultSpace = EditorGUIUtility.singleLineHeight;

        protected readonly string m_TabName;
        protected readonly EditorWindow m_Window;


        public ToolbarEditorTab(EditorWindow window, string tabName)
        {
            this.m_Window = window;
            this.m_TabName = tabName;
        }

        public abstract void Draw();
    }
}
