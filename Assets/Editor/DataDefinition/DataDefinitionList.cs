using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    #region Internal
    public class DefinitionActionWindow : EditorWindow
    {
        private static DefinitionActionWindow s_Window;
        private static EditorWindow s_LastFocusedWindow;

        private Action<string> m_CreateDefinition;

        private string m_DefinitionName;
        private string m_DefinitionTypeName;
        private string m_ActionName;


        public static void OpenWindow(Action<string> action, string typeName, string actionName)
        {
            if (s_Window != null)
                s_Window.Close();

            s_LastFocusedWindow = focusedWindow;

            typeName = typeName.Replace("Definition", "").ToUnityLikeNameFormat();
            s_Window = GetWindow<DefinitionActionWindow>(utility: true, title: $"{actionName} {typeName}", focus: true);

            s_Window.maxSize = new Vector2(512, 84);
            s_Window.minSize = new Vector2(512, 84);

            s_Window.m_CreateDefinition = action;
            s_Window.m_DefinitionTypeName = typeName;
            s_Window.m_ActionName = actionName;
        }

        private void OnGUI()
        {
            if (Event.current.keyCode == KeyCode.Escape)
            {
                CloseWindow();
                return;
            }

            GUILayout.FlexibleSpace();

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                GUILayout.Label(m_DefinitionTypeName);

                GUI.SetNextControlName("DefText");
                m_DefinitionName = EditorGUILayout.TextField(m_DefinitionName);

                GUILayout.FlexibleSpace();
            }

            GUILayout.FlexibleSpace();

            EditorGUI.FocusTextInControl("DefText");

            if ((Event.current.keyCode == KeyCode.Return) || GUILayout.Button($"{m_ActionName} {m_DefinitionTypeName}", CustomGUIStyles.LargeButton))
            {
                m_CreateDefinition?.Invoke(m_DefinitionName);
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

    public class ListAction
    {
        private readonly Action m_Click;
        private readonly Func<bool> m_Enabled;
        private readonly GUIContent m_GUIContent;
        private readonly Color m_Color;
        private readonly KeyCode m_ShortcutKey;


        public ListAction(Action click, Func<bool> enabled, string text, string iconPath, string tooltip, KeyCode shortcutKey, Color color = default)
        {
            m_Click = click;
            m_Enabled = enabled;
            m_GUIContent = new GUIContent(text, Resources.Load<Texture2D>(iconPath), tooltip);
            m_ShortcutKey = shortcutKey;
            m_Color = color;
        }

        public void DrawGUI(GUILayoutOption[] buttonLayout = null)
        {
            GUI.enabled = m_Enabled();

            if (CustomGUILayout.ColoredButton(m_GUIContent, m_Color, buttonLayout))
                m_Click?.Invoke();

            GUI.enabled = true;
        }

        public void HandleEvent(Event current)
        {
            if (current.control && current.keyCode == m_ShortcutKey)
                m_Click?.Invoke();
        }
    }

    public abstract class DataDefinitionListBase
    {
        public abstract int Count { get; }
        public string ListName => m_ListName;

        protected string m_ListName;
        protected static DataDefinitionBase s_Copy;


        public DataDefinitionListBase(string listName)
        {
            m_ListName = listName;
        }

        public abstract void DrawGUI();

        #region Focusing
        private static DataDefinitionListBase s_FocusedList;

        protected bool HasFocus() => s_FocusedList == this;
        protected void FocusList(DataDefinitionListBase list) => s_FocusedList = list;
        protected void RemoveFocus()
        {
            if (s_FocusedList == this)
                s_FocusedList = null;
        }
        #endregion
    }
    #endregion

    public abstract class  DataDefinitionList<T> : DataDefinitionListBase where T : DataDefinition<T>
    {
        public Func<T[]> GetDefinitions { get; set; }
        public bool IsInteractable { get; set; } = true;

        public override int Count => Definitions != null ? Definitions.Count : 0;
        public string SelectedName => Selected != null ? Selected.Name : string.Empty;

        public T Selected
        {
            get
            {
                if (Definitions != null && Definitions.Count > 0)
                    return Definitions[SelectedIndex];

                return null;
            }
        }

        public int SelectedIndex
        {
            get => Mathf.Clamp(m_SelectedIndex, 0, Definitions.Count != 0 ? Definitions.Count - 1 : 0);
            private set => m_SelectedIndex = Mathf.Clamp(value, 0, Definitions.Count - 1);
        }

        protected List<T> Definitions { get; private set; }

        public event Action Refreshed;
        public event Action<T> DefinitionSelected;
        public event Action<T> DefinitionCreated;
        public event Action<T> DefinitionDeleted;

        private bool m_IsDirty;
        private int m_SelectedIndex = 0;
        private Vector2 m_ListEditingScrollPosition;

        private readonly ListAction[] m_CustomActions;
        private readonly GUILayoutOption[] m_ButtonSizeOptions;


        public DataDefinitionList(string listName, params ListAction[] customActions) : base (listName)
        {
            m_ListName = listName;
            m_ButtonSizeOptions = new GUILayoutOption[] { GUILayout.Height(24f), GUILayout.Width(40f) };

            Definitions = new List<T>();
            m_CustomActions = customActions;
        }

        public override void DrawGUI()
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F5)
            {
                RefreshDefinitions();
                return;
            }

            if (m_IsDirty)
            {
                RefreshDefinitions();
                m_IsDirty = false;
            }
            else if (Selected != null && Selected.IsDirty())
            {
                m_IsDirty = true;
                RemoveFocus();
                Selected.ClearDirty();
            }
        }

        public void RefreshDefinitions()
        {
            if (GetDefinitions != null)
                SetDefinitions(GetDefinitions());
            else
            {
                DataDefinition<T>.ReloadDefinitions();
                SetDefinitions(DataDefinition<T>.Definitions);
            }

            Refreshed?.Invoke();
        }

        public void SelectIndex(int index)
        {
            SelectedIndex = index;
            DefinitionSelected?.Invoke(Selected);
            FocusList(this);
        }

        public void SelectDefinition(T def)
        {
            int index = GetIndexOfDefinition(def);

            if (index != -1)
                SelectIndex(index);
        }

        protected void CreateDefinition(string defName)
        {
            if (TryCreateDefinitionPath(defName, out string path))
            {
                T newDef = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(newDef, path);

                FocusList(this);
                EditorUtility.SetDirty(newDef);

                newDef.Reset();

                DefinitionCreated?.Invoke(newDef);
                Undo.RegisterCreatedObjectUndo(newDef, "New Definition");

                RefreshDefinitions();
                SelectIndex(GetIndexOfDefinition(newDef));
            }
        }

        protected void DuplicateDefinition(T def, string defName)
        {
            if (def != null && TryCreateDefinitionPath(defName, out string defPath))
            {
                string assetPath = AssetDatabase.GetAssetPath(def);

                if (AssetDatabase.CopyAsset(assetPath, defPath))
                {
                    T newDef = AssetDatabase.LoadAssetAtPath<T>(defPath);

                    FocusList(this);
                    EditorUtility.SetDirty(newDef);

                    newDef.Reset();

                    DefinitionCreated?.Invoke(newDef);
                    Undo.RegisterCreatedObjectUndo(newDef, "New Definition");

                    RefreshDefinitions();
                    SelectIndex(GetIndexOfDefinition(newDef));
                }
            }
        }

        protected bool TryCreateDefinitionPath(string defName, out string defPath)
        {
            string newDefPath;
            string creationPath = DataDefinitionUtility.GetAssetCreationPath<T>();

            if (!AssetDatabase.IsValidFolder(creationPath))
            {
                defPath = string.Empty;
                return false;
            }

            string prefix = DataDefinitionUtility.GetAssetNamePrefix(typeof(T));

            if (string.IsNullOrEmpty(creationPath))
                newDefPath = $"Assets/({prefix}) {defName}.asset";
            else
                newDefPath = $"{creationPath}/({prefix}) {defName}.asset";

            defPath = AssetDatabase.GenerateUniqueAssetPath(newDefPath);

            return true;
        }

        protected void DeleteDefinition(T def)
        {
            if (def == null)
                return;

            int lastIndex = GetIndexOfDefinition(Selected);

            string assetPath = AssetDatabase.GetAssetPath(def); 
            if (AssetDatabase.DeleteAsset(assetPath))
            {
                FocusList(this);

                DefinitionDeleted?.Invoke(def);
                RefreshDefinitions();
                SelectIndex(lastIndex - 1);
            }
        }

        protected void DeleteAllDefinitions()
        {
            for (int i = Count - 1; i > -1; i--)
                DeleteDefinition(Definitions[i]);
        }

        protected int GetIndexOfDefinition(T def)
        {
            for (int i = 0; i < Definitions.Count; i++)
            {
                if (def == Definitions[i])
                    return i;
            }

            return -1;
        }

        protected virtual void SetDefinitions(T[] dataDefs)
        {
            int prevScriptableCount = Count;

            Definitions.Clear();
            for (int i = 0; i < dataDefs.Length; i++)
            {
                if (dataDefs[i] != null)
                    Definitions.Add(Definitions[i]);
            }

            if (prevScriptableCount != Count)
                SelectDefinition(Selected);
        }

        protected void DrawListEditingGUI()
        {
            using (var view = new GUILayout.ScrollViewScope(m_ListEditingScrollPosition))
            {
                m_ListEditingScrollPosition = view.scrollPosition;
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (HandleListEditing())
                    {
                        Event.current.Use();
                        FocusList(this);
                    }
                    GUILayout.FlexibleSpace();
                }
            }

            EditorGUILayout.Space();
        }

        private bool HandleListEditing()
        {
            var current = Event.current;
            bool doAction = HasFocus() && current.type == EventType.KeyDown;
            bool control = current.control;
            KeyCode keyCode = current.keyCode;

            if (doAction && keyCode == KeyCode.DownArrow)
            {
                SelectIndex(m_SelectedIndex + 1);
                return true;
            }
            else if (doAction && keyCode == KeyCode.UpArrow)
            {
                SelectIndex(m_SelectedIndex - 1);
                return true;
            }

            GUI.enabled = IsInteractable;
            if (CustomGUILayout.ColoredButton(CustomGUIStyles.CreateEmptyContent, CustomGUIStyles.GreenColor, m_ButtonSizeOptions) ||
                (doAction && control && keyCode == KeyCode.Space))
            {
                DefinitionActionWindow.OpenWindow(CreateDefinition, typeof(T).Name, "Create");
                return true;
            }

            GUI.enabled = Selected != null;
            if (CustomGUILayout.ColoredButton(CustomGUIStyles.DuplicateSelectedContent, CustomGUIStyles.BlueColor, m_ButtonSizeOptions) ||
                (doAction && control && keyCode == KeyCode.D))
            {
                DefinitionActionWindow.OpenWindow((string name) => DuplicateDefinition(Selected, name), typeof(T).Name, "Duplicate");
                return true;
            }

            EditorGUILayout.Space();

            GUI.enabled = Selected != null;
            if (CustomGUILayout.ColoredButton(CustomGUIStyles.DeleteSelectedContent, CustomGUIStyles.RedColor, m_ButtonSizeOptions) ||
                (doAction && !control && keyCode == KeyCode.Delete))
            {
                if (EditorUtility.DisplayDialog($"Delete element?", $"Delete ''{Selected.Name}''?", "Ok", "Cancel"))
                {
                    DeleteDefinition(Selected);
                    return true;
                }
            }

            GUI.enabled = Count != 0;
            if (CustomGUILayout.ColoredButton(CustomGUIStyles.DeleteAllContent, CustomGUIStyles.RedColor, m_ButtonSizeOptions) ||
                (doAction && control && keyCode == KeyCode.Delete))
            {
                if (EditorUtility.DisplayDialog("Delete all elements", $"Delete all of the elements from this list?", "Ok", "Cancel"))
                {
                    DeleteAllDefinitions();
                    return true;
                }
            }

            GUI.enabled = true;

            if (doAction && control && keyCode == KeyCode.C)
            {
                s_Copy = Selected;
                EditorGUIUtility.systemCopyBuffer = string.Empty;
                return true;
            }

            if (EditorGUIUtility.systemCopyBuffer == string.Empty && s_Copy != null)
            {
                if (doAction && control && keyCode == KeyCode.V)
                {
                    DuplicateDefinition((T)s_Copy, (T)s_Copy != null ? s_Copy.Name : string.Empty);
                    s_Copy = null;
                    return true;
                }
            }

            EditorGUILayout.Space();

            foreach (var action in m_CustomActions)
            {
                action.DrawGUI(m_ButtonSizeOptions);

                if (doAction)
                {
                    action.HandleEvent(current);
                    return true;
                }
            }

            return false;
        }
    }
}