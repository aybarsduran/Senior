using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    public class DataDefinitionTools : ScriptableObject { }

    [CustomEditor(typeof(DataDefinitionTools), true)]
    public class DataDefinitionToolsEditor : ToolboxEditor
    {
        protected override bool DrawScriptProperty => false;


        protected override void DrawCustomInspector()
        {
            base.DrawCustomInspector();

            GUILayout.Space(12f);

            if (ToolbarEditorWindowBase.HasActiveWindows)
                DrawDataDefinitionShortcuts();

            GUILayout.FlexibleSpace();

            if (CustomGUILayout.ColoredButton("Fix All Asset Definition Names", CustomGUIStyles.BlueColor, GUILayout.Height(25f)))
                DataDefinitionUtility.ResetAllAssetDefinitionNames();
        }

        private void DrawDataDefinitionShortcuts()
        {
            // Display the shortcuts
            var shortcutStyle = new GUIStyle(CustomGUIStyles.BoldMiniGreyLabel) { alignment = TextAnchor.MiddleLeft, fontSize = 13 };

            GUILayout.Label("Shortcuts");
            CustomGUILayout.Separator(Color.white * 0.4f);

            DrawShortcut("'F5'", "Refresh database.", shortcutStyle);
            DrawShortcut("'Up / Down' Arrows", "Navigate Selection.", shortcutStyle);
            DrawShortcut("'Ctrl + Space'", "Create a new element.", shortcutStyle);
            DrawShortcut("'Ctrl + D'", "Duplicate an element.", shortcutStyle);
            DrawShortcut("'Ctrl + C'", "Copy element.", shortcutStyle);
            DrawShortcut("'Ctrl + V'", "Paste element.", shortcutStyle);
            DrawShortcut("'Del'", "Delete a single element.", shortcutStyle);
            DrawShortcut("'Ctrl + Del'", "Delete all elements.", shortcutStyle);
            DrawShortcut("'Ctrl + U'", "Unlink element.", shortcutStyle);
        }

        private void DrawShortcut(string shortcut, string label, GUIStyle labelStyle)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label(shortcut, labelStyle);
            GUILayout.Label(label);
            GUILayout.EndVertical();
        }
    }
}
