using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    public static class CustomGUIStyles
    {
        #region Custom Colors

        public static Color SplitterColor => EditorGUIUtility.isProSkin ? new Color(0.2f, 0.2f, 0.2f) : new Color(0.5f, 0.5f, 0.5f);
        public static Color BlueColor => EditorGUIUtility.isProSkin ? new Color(0.8f, 0.92f, 1.065f, 0.78f) : new Color(0.9f, 0.97f, 1.065f, 0.75f);
        public static Color GreenColor => EditorGUIUtility.isProSkin ? new Color(0.5f, 1f, 0.5f, 0.75f) : new Color(0.5f, 1f, 0.5f, 0.75f);
        public static Color RedColor => EditorGUIUtility.isProSkin ? new Color(1f, 0.5f, 0.5f, 0.75f) : new Color(1f, 0.5f, 0.5f, 0.75f);
        public static Color LightRedColor => EditorGUIUtility.isProSkin ? new Color(1f, 0.65f, 0.65f, 0.75f) : new Color(1f, 0.65f, 0.65f, 0.75f);
        public static Color YellowColor => EditorGUIUtility.isProSkin ? new Color(1f, 1f, 0.8f, 0.75f) : new Color(1f, 1f, 0.8f, 0.75f);

        private static Color DefaultTextColor => EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.65f) : new Color(0.1f, 0.1f, 0.1f, 0.85f);
        private static Color HighlightedTextColor => EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.9f) : new Color(0.1f, 0.1f, 0.1f, 1f);

        #endregion

        #region Custom Styles
        private static GUIStyle s_Title;
        public static GUIStyle Title
        {
            get
            {
                if (s_Title == null)
                {
                    s_Title = new(EditorStyles.toolbar);
                    s_Title.fontSize = 12;
                    s_Title.alignment = TextAnchor.MiddleCenter;
                    s_Title.normal.textColor *= 1.1f;
                }
                return s_Title;
            }
        }

        private static GUIStyle s_LargeTitleLabel;
        public static GUIStyle LargeTitleLabel
        {
            get
            {
                if (s_LargeTitleLabel == null)
                {
                    s_LargeTitleLabel = new(EditorStyles.boldLabel);
                    s_LargeTitleLabel.fontSize = 12;
                    s_LargeTitleLabel.normal.textColor = new Color(1, 1, 1, 0.65f);
                    s_LargeTitleLabel.alignment = TextAnchor.MiddleCenter;
                }
                return s_LargeTitleLabel;
            }
        }

        private static GUIStyle s_MediumTitleLabel;
        public static GUIStyle MediumTitleLabel
        {
            get
            {
                if (s_MediumTitleLabel == null)
                {
                    s_MediumTitleLabel = new(LargeTitleLabel);
                    s_MediumTitleLabel.fontSize = 11;
                    s_MediumTitleLabel.padding.left += 6;
                    s_MediumTitleLabel.alignment = TextAnchor.MiddleLeft;
                }
                return s_MediumTitleLabel;
            }
        }

        private static GUIStyle s_SmallTitleLabel;
        public static GUIStyle SmallTitleLabel
        {
            get
            {
                if (s_SmallTitleLabel == null)
                {
                    s_SmallTitleLabel = new(CenteredMiniLabel);
                    s_SmallTitleLabel.alignment = TextAnchor.MiddleLeft;
                    s_SmallTitleLabel.padding.left = 6;
                    s_SmallTitleLabel.padding.right = 6;
                }
                return s_SmallTitleLabel;
            }
        }

        private static GUIStyle s_CenteredMiniLabel;
        public static GUIStyle CenteredMiniLabel
        {
            get
            {
                if (s_CenteredMiniLabel == null)
                {
                    s_CenteredMiniLabel = new(EditorStyles.centeredGreyMiniLabel);
                    s_CenteredMiniLabel.normal.textColor = DefaultTextColor;
                    s_CenteredMiniLabel.wordWrap = true;
                    s_CenteredMiniLabel.fontSize = 11;
                    s_CenteredMiniLabel.padding.left += 16;
                    s_CenteredMiniLabel.padding.right += 16;
                }
                return s_CenteredMiniLabel;
            }
        }

        private static GUIStyle s_BoldMiniGreyLabel;
        public static GUIStyle BoldMiniGreyLabel
        {
            get
            {
                if (s_BoldMiniGreyLabel == null)
                {
                    s_BoldMiniGreyLabel = new(EditorStyles.centeredGreyMiniLabel);
                    s_BoldMiniGreyLabel.fontStyle = FontStyle.Bold;
                }
                return s_BoldMiniGreyLabel;
            }
        }

        private static GUIStyle s_StandardFoldout;
        public static GUIStyle StandardFoldout
        {
            get
            {
                if (s_StandardFoldout == null)
                {
                    s_StandardFoldout = new(EditorStyles.foldout);
                    s_StandardFoldout.fontStyle = FontStyle.Italic;
                    s_StandardFoldout.normal.textColor = DefaultTextColor;
                    s_StandardFoldout.fontSize = 11;
                }
                return s_StandardFoldout;
            }
        }

        private static GUIStyle s_ButtonFoldout;
        public static GUIStyle ButtonFoldout
        {
            get
            {
                if (s_ButtonFoldout == null)
                {
                    s_ButtonFoldout = new(CenteredMiniLabel);
                    s_ButtonFoldout.alignment = TextAnchor.MiddleLeft;
                    s_ButtonFoldout.fontStyle = FontStyle.Italic;
                    s_ButtonFoldout.normal.textColor = DefaultTextColor;
                    s_ButtonFoldout.hover.textColor = HighlightedTextColor;
                    s_ButtonFoldout.focused.textColor = HighlightedTextColor;
                    s_ButtonFoldout.fontSize = 12;
                }
                return s_ButtonFoldout;
            }
        }

        private static GUIStyle s_StandardButton;
        public static GUIStyle StandardButton
        {
            get
            {
                if (s_StandardButton == null)
                {
                    s_StandardButton = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).button;
                    s_StandardButton.fontStyle = FontStyle.Normal;
                    s_StandardButton.alignment = TextAnchor.MiddleCenter;
                    s_StandardButton.padding = new RectOffset(5, 0, 2, 2);
                    s_StandardButton.fontSize = 12;
                    s_StandardButton.normal.textColor = new Color(1f, 1f, 1f, 0.85f);
                }
                return s_StandardButton;
            }
        }

        private static GUIStyle s_LargeButton;
        public static GUIStyle LargeButton
        {
            get
            {
                if (s_LargeButton == null)
                {
                    s_LargeButton = new(StandardButton);
                    s_LargeButton.padding.top = 6;
                    s_LargeButton.padding.bottom = 6;
                }
                return s_LargeButton;
            }
        }

        private static GUIStyle s_ColoredButton;
        public static GUIStyle ColoredButton
        {
            get
            {
                if (s_ColoredButton == null)
                {
                    s_ColoredButton = new(StandardButton);
                    s_ColoredButton.onNormal.textColor = YellowColor;
                    s_ColoredButton.onHover.textColor = YellowColor;
                }
                return s_ColoredButton;
            }
        }

        private static GUIStyle s_Splitter;
        public static GUIStyle Splitter
        {
            get
            {
                if (s_Splitter == null)
                {
                    s_Splitter = new();
                    s_Splitter.normal.background = EditorGUIUtility.whiteTexture;
                    s_Splitter.stretchWidth = true;
                    s_Splitter.margin = new RectOffset(0, 0, 5, 5);
                }
                return s_Splitter;
            }
        }

        #endregion

        #region Custom Contents
        public static GUIContent CreateEmptyContent => s_CreateEmptyContent;
        private static readonly GUIContent s_CreateEmptyContent = new(Resources.Load<Texture2D>("Icons/Add"), "Create Empty");

        public static GUIContent DuplicateSelectedContent => s_DuplicateSelectedContent;
        private static readonly GUIContent s_DuplicateSelectedContent = new(Resources.Load<Texture2D>("Icons/Duplicate"), "Duplicate Selected");

        public static GUIContent DeleteSelectedContent => s_DeleteSelectedContent;
        private static readonly GUIContent s_DeleteSelectedContent = new(Resources.Load<Texture2D>("Icons/Delete"), "Delete Selected");

        public static GUIContent DeleteAllContent => s_DeleteAllContent;
        private static readonly GUIContent s_DeleteAllContent = new(Resources.Load<Texture2D>("Icons/DeleteAll"), "Delete All");
        #endregion
    }
}