using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    public abstract class ToolbarEditorWindowBase : EditorWindow
    {
        public static bool HasActiveWindows => s_ActiveWindows != null && s_ActiveWindows.Count > 0;

        private readonly static List<ToolbarEditorWindowBase> s_ActiveWindows = new List<ToolbarEditorWindowBase>();


        protected virtual void OnEnable() => s_ActiveWindows.Add(this);
        protected virtual void OnDisable() => s_ActiveWindows.Remove(this);
    }

    public abstract class ToolbarEditorWindow<T> : ToolbarEditorWindowBase where T : ToolbarEditorWindow<T>
    {
        public static bool IsActive => s_Window != null;
        protected virtual bool ToolbarMatchWindowWidth { get; } = false;
        protected virtual float ToolbarWidth { get; } = 500f;
        protected virtual float ToolbarHeight { get; } = 30f;

        protected int m_ActiveTabIndex = 0;

        protected string[] m_TabNames;
        protected ToolbarEditorTab[] m_Tabs;

        protected static ToolbarEditorWindow<T> s_Window;
        protected readonly Color m_BackgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.85f);


        protected static void CreateToolbarWindow(string windowName, float width = 1500, float height = 800, float minWidth = 1000, float minHeight = 500)
        {
            var x = (Screen.currentResolution.width - width) / 2;
            var y = (Screen.currentResolution.height - height) / 2;

            s_Window = GetWindow<T>(false, windowName);
            s_Window.minSize = new Vector2(minWidth, minHeight);
            s_Window.position = new Rect(x, y, width, height);
        }

        protected abstract ToolbarEditorTab[] GetTabs();

        protected string[] GetTabNames()
        {
            string[] tabNames = new string[m_Tabs.Length];

            for (int i = 0; i < m_Tabs.Length; i++) 
                tabNames[i] = m_Tabs[i].TabName;

            return tabNames;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Tabs = GetTabs();
            m_TabNames = GetTabNames();
        }

        private void OnGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Can't edit or see definitions while in play mode.", MessageType.Warning);
                return;
            }

            if (m_Tabs == null || m_Tabs.Length == 0)
                return;

            s_Window = this;

            GUILayout.Space(5f);

            if (!ToolbarMatchWindowWidth)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
            }

            m_ActiveTabIndex = GUILayout.Toolbar(m_ActiveTabIndex, m_TabNames, GetGUILayoutOptions());

            if (!ToolbarMatchWindowWidth)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUI.color = m_BackgroundColor;
            GUILayout.BeginVertical("Box");
            GUI.color = Color.white;

            GUILayout.Space(10f);

            m_Tabs[m_ActiveTabIndex].Draw();

            GUILayout.EndVertical();
        }

        private GUILayoutOption[] GetGUILayoutOptions()
        {
            if (ToolbarMatchWindowWidth)
            {
                return new GUILayoutOption[] 
                {
                    GUILayout.Height(30f)
                };
            }
            else
            {
                return new GUILayoutOption[]
                {
                    GUILayout.Height(30f),
                    GUILayout.Width(ToolbarWidth)
                };
            }
        }
    }
}
