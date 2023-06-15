using System;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    public class GroupDefinitionEditorTab<Group, Member> : ToolbarEditorTab where Group : GroupDefinition<Group, Member> where Member : GroupMemberDefinition<Member, Group>
    {
        public float GroupListWidthPercent
        {
            get => m_GroupListWidth;
            set => m_GroupListWidth = Mathf.Clamp01(value);
        }

        public float MembersListWidthPercent
        {
            get => m_MembersListWidth;
            set => m_MembersListWidth = Mathf.Clamp01(value);
        }

        public float MemberInspectorWidthPercent
        {
            get => m_MemberInspectorWidth;
            set => m_MemberInspectorWidth = Mathf.Clamp01(value);
        }

        private float m_GroupListWidth = 0.17f;
        private float m_MembersListWidth = 0.21f;
        private float m_MemberInspectorWidth = 0.57f;

        // Toolbars...
        protected readonly SearchableDataDefinitionToolbar<Member> m_MembersToolbar;
        protected readonly SearchableDataDefinitionToolbar<Group> m_GroupsToolbar;

        // Inspectors...
        protected readonly ObjectInspectorDrawer m_MemberInspector;
        protected readonly ObjectInspectorDrawer m_GroupInspector;

        // Members Unlinker...
        protected readonly DataDefinitionUnlinker<Group, Member> m_Unlinker;


        #region Logic
        public GroupDefinitionEditorTab(EditorWindow window, string tabName, string groupsToolbarName = "Categories", string membersToolbarName = "Items") : base(window, tabName)
        {
            // Create the member unlinker.
            m_Unlinker = new DataDefinitionUnlinker<Group, Member>(GUILayout.Height(258f));
            m_Unlinker.DefinitionAdded += OnMemberLinked;

            // Create the groups toolbar and inspector.
            var mergeAction = new ListAction(MergeGroup, CanMerge, "", "Icons/Unlink", "Merge Selected", KeyCode.None, CustomGUIStyles.BlueColor);
            foreach (var def in GroupDefinition<Group, Member>.Definitions)
                def.RemoveAllNullMembers();

            m_GroupsToolbar = new SearchableDataDefinitionToolbar<Group>(groupsToolbarName, mergeAction)
            {
                ButtonHeight = 35f,
                IconSize = 0.7f
            };

            m_GroupsToolbar.Refreshed += m_Unlinker.Refresh;
            m_GroupsToolbar.DefinitionSelected += OnGroupSelected;
            m_GroupsToolbar.DefinitionDeleted += OnGroupDeleted;

            m_GroupInspector = new ObjectInspectorDrawer(m_GroupsToolbar.Selected, GUILayout.Height(260f))
            {
                ShowInspector = true
            };

            // Create the items toolbar and inspector.
            var unlinkAction = new ListAction(UnlinkMember, CanUnlink, "", "Icons/Unlink", "Unlink Selected", KeyCode.U, CustomGUIStyles.YellowColor);
            m_MembersToolbar = new SearchableDataDefinitionToolbar<Member>(membersToolbarName, unlinkAction)
            {
                ButtonHeight = 45f, // Default Value
                IconSize = 0.9f
            };

            m_MemberInspector = new ObjectInspectorDrawer(m_MembersToolbar.Selected);

            m_MembersToolbar.Refreshed += m_Unlinker.Refresh;
            m_MembersToolbar.DefinitionSelected += m_MemberInspector.SetObject;
            m_MembersToolbar.DefinitionCreated += OnMemberCreated;
            m_MembersToolbar.DefinitionDeleted += OnMemberDeleted;
            m_MembersToolbar.GetDefinitions = GetMembers;

            m_GroupsToolbar.RefreshDefinitions();
            m_MembersToolbar.RefreshDefinitions();


            #region Local Methods
            void OnGroupSelected(Group group)
            {
                if (!m_GroupsToolbar.IsSearching)
                    GUI.FocusControl(null);

                m_MembersToolbar.RefreshDefinitions();

                m_GroupInspector.SetObject(group);
                m_MemberInspector.SetObject(m_MembersToolbar.Selected); 
            }

            void OnGroupDeleted(Group group)
            {
                if (group == null || group.Members.Length == 0)
                    return;

                if (EditorUtility.DisplayDialog("Delete all child members", $"Delete all of the child members from this group?", "Ok", "Unlink"))
                {
                    foreach (var item in group.Members)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(item);
                        AssetDatabase.DeleteAsset(assetPath);
                    }
                }
            }

            void MergeGroup()
            {
                var groups = DataDefinitionUtility.GetAllGUIContents<Group>(true, true, true, null);
                GroupMergeWindow.OpenWindow(groups, Merge, m_GroupsToolbar.SelectedIndex);

                void Merge(int index)
                {
                    var group1 = m_GroupsToolbar.Selected;
                    var group2 = DataDefinition<Group>.GetWithIndex(index);
                    group1.MergeWith(group2);

                    m_GroupsToolbar.RefreshDefinitions();
                    m_GroupsToolbar.SelectDefinition(group1);
                }
            }

            bool CanMerge()
            {
                return m_GroupsToolbar.Selected != null && m_GroupsToolbar.Count > 1;
            }

            void OnMemberCreated(Member member)
            {
                m_GroupsToolbar.Selected.AddMember(member);
                m_GroupsToolbar.Selected.AddDefaultDataToDefinition(member);

                m_MembersToolbar.RefreshDefinitions();
            }

            void OnMemberDeleted(Member member)
            {
                m_GroupsToolbar.Selected.RemoveAllNullMembers();
            }

            void UnlinkMember()
            {
                m_GroupsToolbar.Selected.RemoveMember(m_MembersToolbar.Selected);
                m_MembersToolbar.RefreshDefinitions();
            }

            bool CanUnlink()
            {
                return m_MembersToolbar.Selected != null;
            }

            void OnMemberLinked(Member member)
            {
                OnMemberCreated(member);
                m_MembersToolbar.SelectDefinition(member);
            }

            Member[] GetMembers()
            {
                if (m_GroupsToolbar.Selected == null)
                {
                    m_MembersToolbar.IsInteractable = false;
                    return null;
                }

                m_MembersToolbar.IsInteractable = true;
                return m_GroupsToolbar.Selected.Members;
            }
            #endregion
        }
        #endregion

        #region Drawing
        public override void Draw()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                DrawGroupsList();

                GUILayout.FlexibleSpace();
                DrawMembersList();
                GUILayout.FlexibleSpace();

                DrawMemberInspector();
                GUILayout.FlexibleSpace();
            }
        }

        private void DrawGroupsList()
        {
            var width = GUILayout.Width(Rect.width * m_GroupListWidth);
            EditorTabUtility.DrawVerticalToolbarList(m_GroupsToolbar, "", width, DrawGroupInspector);
        }

        private void DrawGroupInspector(GUILayoutOption width)
        {
            EditorTabUtility.DrawObjectInspectorWithToggle(m_GroupInspector, m_GroupsToolbar.SelectedName, width);
        }

        private void DrawMembersList()
        {
            var width = GUILayout.Width(Rect.width * m_MembersListWidth);
            EditorTabUtility.DrawVerticalToolbarList(m_MembersToolbar, m_GroupsToolbar.SelectedName, width, DrawMemberUnlinker);
        }

        private void DrawMemberUnlinker(GUILayoutOption width)
        {
            if (m_GroupsToolbar.Selected != null && m_Unlinker.HasUnlinkedDefinitions)
                EditorTabUtility.DrawObjectInspectorWithToggle(ref m_Unlinker.DrawInspector, m_Unlinker.DrawGUI, $"Unlinked {m_MembersToolbar.ListName}", width);
            else
                m_Unlinker.DrawInspector = false;
        }

        private void DrawMemberInspector()
        {
            var width = GUILayout.Width(Rect.width * m_MemberInspectorWidth);
            EditorTabUtility.DrawSimpleObjectInspector(m_MemberInspector, m_MembersToolbar.SelectedName, width);
        }
        #endregion
    }

    internal class GroupMergeWindow : EditorWindow
    {
        private static GroupMergeWindow s_Window;
        private static EditorWindow s_LastFocusedWindow;

        private GUIContent[] m_Groups;
        private Action<int> m_CombineAction;
        private int m_SelectedGroup;
        private int m_TargetGroup;

        public static void OpenWindow(GUIContent[] groups, Action<int> mergeAction, int selectedGroup)
        {
            if (s_Window != null)
                s_Window.Close();

            s_LastFocusedWindow = focusedWindow;

            s_Window = GetWindow<GroupMergeWindow>(utility: true, title: $"Merge Groups", focus: true);

            s_Window.minSize = new Vector2(256, 32 * groups.Length + 64);
            s_Window.maxSize = s_Window.minSize;

            s_Window.m_Groups = groups;
            s_Window.m_CombineAction = mergeAction;
            s_Window.m_SelectedGroup = selectedGroup;
        }

        private void OnGUI()
        {
            if (Event.current.keyCode == KeyCode.Escape)
            {
                CloseWindow();
                return;
            }

            m_TargetGroup = GUILayout.SelectionGrid(m_TargetGroup, m_Groups, 1, GUILayout.Height(32 * m_Groups.Length));

            GUILayout.FlexibleSpace();

            GUI.enabled = m_TargetGroup != m_SelectedGroup;

            if ((Event.current.keyCode == KeyCode.Return) || GUILayout.Button($"Merge ''{m_Groups[m_TargetGroup]}'' into ''{m_Groups[m_SelectedGroup]}''", CustomGUIStyles.LargeButton))
            {
                m_CombineAction?.Invoke(m_TargetGroup);
                CloseWindow();
            }
        }

        private void CloseWindow()
        {
            s_Window.Close();

            if (s_LastFocusedWindow != null)
                s_LastFocusedWindow.Focus();
        }
    }
}
