using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    public class SearchableDataDefinitionToolbar<T> : SearchableDataDefinitionList<T> where T : DataDefinition<T>
    {
        public GUILayoutOption[] RectLayoutOptions { get; set; }
        public float ButtonHeight { get; set; } = 40f;
        public float IconSize { get; set; } = 1f;

        private GUIContent[] m_GUIContents;

        // GUI Styles
        private Vector2 m_ScrollPos;
        private Color m_ButtonColor;
        private GUIStyle m_ButtonStyle;


        #region Initialization
        public SearchableDataDefinitionToolbar(string listName, params ListAction[] customActions) : base(listName, customActions) { }

        protected override void SetDefinitions(T[] scriptables)
        {
            base.SetDefinitions(scriptables);

            m_GUIContents = new GUIContent[Definitions.Count];
            for (int i = 0; i < Definitions.Count; i++)
            {
                m_GUIContents[i] = new GUIContent()
                {
                    text = Definitions[i].Name,
                    tooltip = Definitions[i].Description,
                    image = Definitions[i].Icon != null ? AssetPreview.GetAssetPreview(Definitions[i].Icon) : null
                };
            }
        }
        #endregion

        #region Layout Drawing
        public void SetToolbarStyle(Color buttonColor = default, GUIStyle buttonStyle = null, TextAnchor buttonTextAnchor = TextAnchor.MiddleCenter)
        {
            m_ButtonColor = buttonColor == default ? (CustomGUIStyles.YellowColor + Color.white * 7f) / 8f : buttonColor;
            m_ButtonStyle = buttonStyle == null ? m_ButtonStyle = CustomGUIStyles.ColoredButton : m_ButtonStyle;
            m_ButtonStyle.alignment = buttonTextAnchor;
        }

        public override void DrawGUI()
        {
            base.DrawGUI();

            bool focusThisList = false;
            m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos, RectLayoutOptions);

            DrawSearchBar();
            focusThisList |= HasMouseFocus();

            DrawScriptableToolbar();
            focusThisList |= HasMouseFocus();

            GUILayout.EndScrollView();    
            GUILayout.FlexibleSpace();

            DrawListEditingGUI();

            if (focusThisList)
                FocusList(this);
            else if (Event.current.type == EventType.Repaint)
                RemoveFocus();
        }

        private bool HasMouseFocus()
        {
            return Event.current.type == EventType.Repaint &&
                GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
        }

        private void DrawScriptableToolbar()
        {
            float iconSize = ButtonHeight * IconSize;
            using (new EditorGUIUtility.IconSizeScope(new(iconSize, iconSize)))
            {
                if (Definitions == null || Definitions.Count == 0)
                return;

                if (m_ButtonStyle == null)
                    SetToolbarStyle();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                GUI.backgroundColor = m_ButtonColor;

                int prevIndex = SelectedIndex;
                int newIndex = GUILayout.SelectionGrid(prevIndex, m_GUIContents, 1, m_ButtonStyle, GUILayout.Height(ButtonHeight * Definitions.Count));

                if (prevIndex != newIndex)
                {
                    GUI.FocusControl(null);
                    SelectIndex(newIndex);
                }

                GUI.backgroundColor = Color.white;

                EditorGUILayout.EndVertical();
            }
        }
        #endregion
    }
}