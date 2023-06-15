using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    public class GenericEditorTab : ToolbarEditorTab
    {
        private readonly ObjectInspectorDrawer m_InspectorDrawer;
        private float m_TabWidth;


        public GenericEditorTab(EditorWindow window, string tabName, Object obj, float tabWidth = 0.75f) : base(window, tabName)
        {
            this.m_InspectorDrawer = new ObjectInspectorDrawer(obj);
            this.m_TabWidth = tabWidth;
        }

        public override void Draw()
        {
            float rectWidth = Rect.width * m_TabWidth;
            var width = GUILayout.Width(rectWidth);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                using (new GUILayout.VerticalScope("Box", width))
                {
                    using (new GUILayout.VerticalScope())
                    {
                        GUILayout.Label(m_TabName, CustomGUIStyles.Title, width);
                    }

                    if (m_InspectorDrawer != null)
                        m_InspectorDrawer.DrawGUI();
                }

                GUILayout.FlexibleSpace();
            }
        }
    }
}
