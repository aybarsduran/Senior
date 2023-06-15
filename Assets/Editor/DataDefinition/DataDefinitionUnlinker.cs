using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    public class DataDefinitionUnlinker<Group, Member> where Group : GroupDefinition<Group, Member> where Member : GroupMemberDefinition<Member, Group>
    {
        public bool HasUnlinkedDefinitions => m_UnlinkedDefinitions.Count > 0;

        public GUILayoutOption[] RectLayoutOptions;
        public bool DrawInspector = false;

        public event Action<Member> DefinitionAdded;

        private readonly List<Member> m_UnlinkedDefinitions = new List<Member>();

        private Vector2 m_ScrollPos;
        private GUIContent[] m_GUIContents;
        private int m_SelectedIndex;

        private readonly GUIContent m_ButtonGUI;


        public DataDefinitionUnlinker(params GUILayoutOption[] options)
        {
            RectLayoutOptions = options;
            m_ButtonGUI = new GUIContent("Link Item", Resources.Load<Texture2D>("Icons/Link"), "Link item to selected category");
        }

        public void DrawGUI()
        {
            using (new GUILayout.VerticalScope(RectLayoutOptions))
            {
                using (new EditorGUIUtility.IconSizeScope(new Vector2(32f, 32f)))
                {
                    using (var scroll = new GUILayout.ScrollViewScope(m_ScrollPos))
                    {
                        m_ScrollPos = scroll.scrollPosition;

                        m_SelectedIndex = GUILayout.SelectionGrid(m_SelectedIndex, m_GUIContents, 1, CustomGUIStyles.ColoredButton, GUILayout.Height(30 * m_UnlinkedDefinitions.Count));
                        m_SelectedIndex = Mathf.Clamp(m_SelectedIndex, 0, m_UnlinkedDefinitions.Count - 1);
                    }
                }

                GUILayout.FlexibleSpace();

                if (m_UnlinkedDefinitions.Count > 0)
                {
                    GUILayout.Space(10f);

                    GUILayout.FlexibleSpace();

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        if (CustomGUILayout.ColoredButton(m_ButtonGUI, CustomGUIStyles.BlueColor, GUILayout.Height(30f)))
                            DefinitionAdded?.Invoke(m_UnlinkedDefinitions[m_SelectedIndex]);
                        GUILayout.FlexibleSpace();
                    }

                    GUILayout.Space(3f);
                }
            }
        }

        public void Refresh()
        {
            m_UnlinkedDefinitions.Clear();
            foreach (var def in GroupMemberDefinition<Member, Group>.Definitions)
            {
                if (!def.IsPartOfGroup)
                    m_UnlinkedDefinitions.Add(def);
            }

            m_GUIContents = new GUIContent[m_UnlinkedDefinitions.Count];
            for (int i = 0; i < m_UnlinkedDefinitions.Count; i++)
            {
                m_GUIContents[i] = new GUIContent()
                {
                    text = m_UnlinkedDefinitions[i].Name,
                    tooltip = m_UnlinkedDefinitions[i].Name,
                    image = m_UnlinkedDefinitions[i].Icon != null ? AssetPreview.GetAssetPreview(m_UnlinkedDefinitions[i].Icon) : null
                };
            }
        }
    }
}
