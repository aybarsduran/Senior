using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    public class DataDefinitionsEditorTab: ToolbarEditorTab
    {
        #region Internal
        public class Pair<T> : Pair where T : DataDefinition<T>
        {
            private readonly SearchableDataDefinitionToolbar<T> m_Toolbar;
            private readonly ObjectInspectorDrawer m_Inspector;
            private readonly LayoutStyle m_LayoutStyle;


            public Pair(string listName = "Items", LayoutStyle layoutStyle = LayoutStyle.Vertical, float buttonHeight = 40f)
            {
                // Create the toolbar and inspector.
                m_Toolbar = new SearchableDataDefinitionToolbar<T>(listName);
                m_Inspector = new ObjectInspectorDrawer(m_Toolbar.Selected);

                m_Toolbar.DefinitionSelected += m_Inspector.SetObject;
                m_Toolbar.ButtonHeight = buttonHeight;

                m_LayoutStyle = layoutStyle;

                m_Toolbar.RefreshDefinitions();
            }

            public override void Draw(float width, float height)
            {
                if (m_LayoutStyle == LayoutStyle.Vertical)
                {
                    EditorTabUtility.DrawVerticalToolbarList(m_Toolbar, "", GUILayout.Width(width), (GUILayoutOption width) =>
                    {
                        EditorTabUtility.DrawObjectInspectorWithToggle(m_Inspector, m_Toolbar.SelectedName, width, 20f);
                    });
                }
                else if (m_LayoutStyle == LayoutStyle.Horizontal)
                {
                    using (new GUILayout.HorizontalScope(GUILayout.Height(height)))
                    {
                        EditorTabUtility.DrawVerticalToolbarList(m_Toolbar, "", GUILayout.Width(width * 0.355f));
                        EditorTabUtility.DrawSimpleObjectInspector(m_Inspector, m_Toolbar.SelectedName, 20f, false, GUILayout.Width(width * 0.666f));
                    }
                }
            }
        }

        public abstract class Pair
        {
            public abstract void Draw(float width, float height);
        }

        public enum LayoutStyle
        {
            Vertical,
            Horizontal
        }
        #endregion

        public float WindowWidthPercent
        {
            get => m_WindowWidth;
            set => m_WindowWidth = Mathf.Clamp01(value);
        }
        
        public LayoutStyle Layout
        {
            get => m_Layout;
            set => m_Layout = value;
        }

        private readonly Pair[] m_Pairs;
        private LayoutStyle m_Layout = LayoutStyle.Horizontal;
        private float m_WindowWidth = 1f;


        #region Logic
        public DataDefinitionsEditorTab(EditorWindow window, string tabName, params Pair[] pairs) : base(window, tabName)
        {
            this.m_Pairs = pairs;
        }
        #endregion

        #region Drawing
        public override void Draw()
        {
            if (m_Layout == LayoutStyle.Horizontal)
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    foreach (var pair in m_Pairs)
                    {
                        GUILayout.Space(6f);
                        pair.Draw((Rect.width * m_WindowWidth / m_Pairs.Length) - 28f - DefaultSpace, Rect.height);
                        GUILayout.Space(6f);
                    }

                    GUILayout.FlexibleSpace();
                }
            }
            else
            {
                using (new GUILayout.VerticalScope())
                {
                    foreach (var pair in m_Pairs)
                        pair.Draw(Rect.width * m_WindowWidth, (Rect.height / m_Pairs.Length) * 0.925f);

                    GUILayout.FlexibleSpace();
                }
            }
        }
        #endregion
    }
}
