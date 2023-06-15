using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    [CustomPropertyDrawer(typeof(NavigationUI), true)]
    /// <summary>
    /// This is a PropertyDrawer for NavigationUI. It is implemented using the standard Unity PropertyDrawer framework.
    /// </summary>
    public class NavigationUIDrawer : PropertyDrawer
    {
        private class Styles
        {
            readonly public GUIContent navigationContent;

            public Styles()
            {
                navigationContent = EditorGUIUtility.TrTextContent("Navigation");
            }
        }

        private static Styles s_Styles = null;

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            if (s_Styles == null)
                s_Styles = new Styles();

            Rect drawRect = pos;
            drawRect.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty navigation = prop.FindPropertyRelative("m_Mode");
            SerializedProperty wrapAround = prop.FindPropertyRelative("m_WrapAround");
            NavigationUI.Mode navMode = GetNavigationUIMode(navigation);

            EditorGUI.PropertyField(drawRect, navigation, s_Styles.navigationContent);

            ++EditorGUI.indentLevel;

            drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            switch (navMode)
            {
                case NavigationUI.Mode.Horizontal:
                case NavigationUI.Mode.Vertical:
                    {
                        EditorGUI.PropertyField(drawRect, wrapAround);
                        drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                    break;
                case NavigationUI.Mode.Explicit:
                    {
                        SerializedProperty selectOnUp = prop.FindPropertyRelative("m_SelectOnUp");
                        SerializedProperty selectOnDown = prop.FindPropertyRelative("m_SelectOnDown");
                        SerializedProperty selectOnLeft = prop.FindPropertyRelative("m_SelectOnLeft");
                        SerializedProperty selectOnRight = prop.FindPropertyRelative("m_SelectOnRight");

                        EditorGUI.PropertyField(drawRect, selectOnUp);
                        drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(drawRect, selectOnDown);
                        drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(drawRect, selectOnLeft);
                        drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(drawRect, selectOnRight);
                        drawRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                    break;
            }

            --EditorGUI.indentLevel;
        }

        static NavigationUI.Mode GetNavigationUIMode(SerializedProperty navigation)
        {
            return (NavigationUI.Mode)navigation.enumValueIndex;
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            SerializedProperty navigation = prop.FindPropertyRelative("m_Mode");
            if (navigation == null)
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            NavigationUI.Mode navMode = GetNavigationUIMode(navigation);

            switch (navMode)
            {
                case NavigationUI.Mode.None:
                    return EditorGUIUtility.singleLineHeight;
                case NavigationUI.Mode.Horizontal:
                case NavigationUI.Mode.Vertical:
                    return 2 * EditorGUIUtility.singleLineHeight + 2 * EditorGUIUtility.standardVerticalSpacing;
                case NavigationUI.Mode.Explicit:
                    return 5 * EditorGUIUtility.singleLineHeight + 5 * EditorGUIUtility.standardVerticalSpacing;
                default:
                    return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }
}
