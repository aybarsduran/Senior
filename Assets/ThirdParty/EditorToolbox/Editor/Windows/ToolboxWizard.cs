﻿using UnityEditor;
using UnityEngine;

namespace Toolbox.Editor.Wizards
{
    using Editor = UnityEditor.Editor;

    public class ToolboxWizard : EditorWindow
    {
        private Editor targetEditor;

        private Vector2 scrollPosition;

        private void OnDestroy()
        {
            DestroyImmediate(targetEditor);
        }

        private void OnGUI()
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollView.scrollPosition;
                EditorGUI.BeginChangeCheck();
                OnWizardGui();
                if (EditorGUI.EndChangeCheck())
                {
                    OnWizardUpdate();
                }
            }

            using (new EditorGUILayout.VerticalScope())
            {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    HandleOtherButtons();
                    GUI.enabled = IsValid;
                    if (HandleCreateButton())
                    {
                        OnWizardCreate();
                        if (CloseOnCreate)
                        {
                            Close();
                            GUIUtility.ExitGUI();
                        }
                    }

                    GUI.enabled = true;
                }

                GUILayout.Space(5);
            }
        }

        private void PrepareEditor()
        {
            if (targetEditor != null)
            {
                return;
            }

            targetEditor = Editor.CreateEditor(this);
            targetEditor.hideFlags = HideFlags.HideAndDontSave;
            if (targetEditor is ToolboxEditor toolboxEditor)
            {
                toolboxEditor.IgnoreProperty(PropertyUtility.Defaults.scriptPropertyName);
            }

            OnWizardUpdate();
        }

        protected virtual void OnWizardCreate()
        { }

        protected virtual void OnWizardUpdate()
        { }

        protected virtual void OnWizardGui()
        {
            PrepareEditor();
            targetEditor.OnInspectorGUI();
        }

        protected virtual bool HandleCreateButton()
        {
            return GUILayout.Button("Create", GUILayout.MinWidth(100));
        }

        protected virtual void HandleOtherButtons()
        { }

        public static T DisplayWizard<T>(string title) where T : ToolboxWizard
        {
            return GetWindow<T>(true, title);
        }

        protected virtual bool IsValid { get; set; } = true;

        protected virtual bool CloseOnCreate => true;
    }
}