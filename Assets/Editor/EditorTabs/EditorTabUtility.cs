using System;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    public static class EditorTabUtility
    {
        private static Color s_BoxColor = new Color(0.7f, 0.7f, 0.7f, 0.8f);


        #region Inspectors
        public static void DrawObjectInspectorWithToggle(ObjectInspectorDrawer objInspector, string labelName, GUILayoutOption width = null, float space = 0f, bool separator = true)
        {
            using (new GUILayout.VerticalScope(width))
            {
                GUILayout.Space(6f);

                if (separator)
                    CustomGUILayout.Separator();

                if (objInspector.ShowInspector)
                {
                    GUILayout.Label($"Inspector ({labelName})", CustomGUIStyles.Title, width);

                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        GUILayout.Space(space);
                        objInspector.DrawGUI();
                        GUILayout.Space(space);
                    }
                }

                string btnName = objInspector.ShowInspector ? "Hide Inspector" : "Show Inspector";
                if (CustomGUILayout.ColoredButton(btnName, CustomGUIStyles.YellowColor))
                    objInspector.ShowInspector = !objInspector.ShowInspector;
            }
        }

        public static void DrawObjectInspectorWithToggle(ref bool active, Action drawAction, string labelName, GUILayoutOption width)
        {
            CustomGUILayout.Separator();

            if (active)
            {
                GUILayout.Label($"({labelName})", CustomGUIStyles.Title, width);

                using (new GUILayout.VerticalScope())
                {
                    drawAction();
                }
            }

            string btnName = active ? $"Hide {labelName}" : $"Show {labelName}";
            if (CustomGUILayout.ColoredButton(btnName, CustomGUIStyles.YellowColor))
                active = !active;
        }

        public static void DrawSimpleObjectInspector(ObjectInspectorDrawer objInspector, string labelName, float space = 0f, bool separator = true, params GUILayoutOption[] options)
        {
            using (new CustomGUILayout.VerticalScope("Box", s_BoxColor, false, true, options))
            {
                if (separator)
                    CustomGUILayout.Separator();

                GUILayout.Label($"Inspector ({labelName})", CustomGUIStyles.Title, options);

                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Space(space);
                    objInspector.DrawGUI();
                    GUILayout.Space(space);
                }
            }
        }

        public static void DrawSimpleObjectInspector(ObjectInspectorDrawer objInspector, string labelName, GUILayoutOption width)
        {
            using (new CustomGUILayout.VerticalScope("Box", s_BoxColor, false, true, width))
            {
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.Label($"Inspector ({labelName})", CustomGUIStyles.Title, width);
                }

                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    objInspector.DrawGUI();
                }
            }
        }
        #endregion

        #region Toolbars
        public static void DrawVerticalToolbarList<T>(SearchableDataDefinitionToolbar<T> toolbar, string selectedName, GUILayoutOption width, Action<GUILayoutOption> drawAction) where T : DataDefinition<T>
        {
            using (new CustomGUILayout.VerticalScope("Box", s_BoxColor, false, true, width))
            {
                using (new GUILayout.VerticalScope())
                {
                    selectedName = selectedName != string.Empty ? $"({selectedName}) " : string.Empty;
                    GUILayout.Label($"{toolbar.ListName} {selectedName}- {toolbar.Count}", CustomGUIStyles.Title, width);
                    toolbar.RectLayoutOptions = new GUILayoutOption[] { width };
                    toolbar.DrawGUI();
                }

                drawAction?.Invoke(width);
            }
        }

        public static void DrawVerticalToolbarList<T>(SearchableDataDefinitionToolbar<T> toolbar, string selectedName, params GUILayoutOption[] options) where T : DataDefinition<T>
        {
            using (new CustomGUILayout.VerticalScope("Box", s_BoxColor, false, true))
            {
                selectedName = selectedName != string.Empty ? $"({selectedName}) " : string.Empty;
                GUILayout.Label($"{toolbar.ListName} {selectedName}- {toolbar.Count}", CustomGUIStyles.Title, options);
                toolbar.RectLayoutOptions = options;
                toolbar.DrawGUI();
            }
        }
        #endregion
    }
}
