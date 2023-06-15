using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    public static class CustomGUILayout
    {
        #region GUI Scopes
        public class VerticalScope : GUI.Scope
        {
            private readonly bool m_AddSeparator;
            private readonly bool m_AddSpace;

            private const float k_Space = 3f;


            public VerticalScope(GUIStyle style, Color color = default, bool addSpace = true, bool addSeparator = true, params GUILayoutOption[] guiLayoutOptions)
            {
                if (color != default)
                    GUI.color = color;

                GUILayout.BeginVertical(style, guiLayoutOptions);
                GUI.color = Color.white;

                m_AddSeparator = addSeparator;
                m_AddSpace = addSpace;

                if (addSpace)
                    GUILayout.Space(k_Space);
            }

            protected override void CloseScope()
            {
                if (m_AddSpace)
                    GUILayout.Space(3f);

                GUILayout.EndVertical();

                if (m_AddSeparator)
                    Separator(k_Space);
            }
        }

        public class HorizontalScope : GUI.Scope
        {
            public HorizontalScope(GUIStyle style, Color color = default, float space = 3f, params GUILayoutOption[] guiLayoutOptions)
            {
                if (color != default)
                    GUI.color = color;

                GUILayout.BeginHorizontal(style, guiLayoutOptions);
                GUI.color = Color.white;

                GUILayout.Space(space);
            }

            protected override void CloseScope()
            {
                GUILayout.Space(3f);
                GUILayout.EndHorizontal();
            }
        }
        #endregion

        public static string k_SearchString = "Search...";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns> True if the search string is not empty.</returns>
        public static bool SearchBar(ref string search)
        {
            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));

            const string searchBarName = "SearchBar";

            GUI.SetNextControlName(searchBarName);
            search = GUILayout.TextField(search, GUI.skin.FindStyle("ToolbarSeachTextField"));
            bool hasFocus = GUI.GetNameOfFocusedControl() == searchBarName;

            if (search == k_SearchString && hasFocus)
                search = string.Empty;

            if (GUILayout.Button(string.Empty, GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                // Remove focus if cleared
                search = k_SearchString;
                GUI.FocusControl(null);
            }

            GUILayout.EndHorizontal();

            return hasFocus;
        }

        public static bool ColoredButton(string label, Color color, params GUILayoutOption[] options)
        {
            GUI.backgroundColor = color;
            bool pressedButton = GUILayout.Button(label, options);
            GUI.backgroundColor = Color.white;

            return pressedButton;
        }

        public static bool ColoredButton(string label, Color color, GUIStyle style, params GUILayoutOption[] options)
        {
            GUI.backgroundColor = color;
            bool pressedButton = GUILayout.Button(label, style, options);
            GUI.backgroundColor = Color.white;

            return pressedButton;
        }

        public static bool ColoredButton(GUIContent content, Color color, params GUILayoutOption[] options)
        {
            GUI.backgroundColor = color;
            bool pressedButton = GUILayout.Button(content, options);
            GUI.backgroundColor = Color.white;

            return pressedButton;
        }

        public static bool ColoredButton(Object texture, Color color, params GUILayoutOption[] options)
        {
            GUI.backgroundColor = color;
            Texture2D assetPreview = AssetPreview.GetAssetPreview(texture);
            bool pressedButton = GUILayout.Button(assetPreview, options);
            GUI.backgroundColor = Color.white;

            return pressedButton;
        }

        public static bool ColoredButton(string label, Object textureObj, params GUILayoutOption[] options)
        {
            var texture = AssetPreview.GetAssetPreview(textureObj);
            GUIContent guiContent = new GUIContent(label, texture);
            bool pressedButton = GUILayout.Button(guiContent, options);

            return pressedButton;
        }

        public static void Separator(Color rgb, float thickness = 1)
        {
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, CustomGUIStyles.Splitter, GUILayout.Height(thickness));

            if (Event.current.type == EventType.Repaint)
            {
                Color restoreColor = GUI.color;
                GUI.color = rgb;
                CustomGUIStyles.Splitter.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }

        public static void Separator(float thickness = 1f)
        {
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, CustomGUIStyles.Splitter, GUILayout.Height(thickness));

            if (Event.current.type == EventType.Repaint)
            {
                Color restoreColor = GUI.color;
                GUI.color = CustomGUIStyles.SplitterColor;
                CustomGUIStyles.Splitter.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }

        public static void Separator(Rect position)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color restoreColor = GUI.color;
                GUI.color = CustomGUIStyles.SplitterColor;
                CustomGUIStyles.Splitter.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }
    }
}
