using Toolbox.Editor.Drawers;
using UnityEditor;
using UnityEngine;

namespace PolymindGames
{
    [CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
    public class SpritePreviewDrawer : PropertyDrawerBase
    {
        const float imageHeight = 64;


        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference &&
                (property.objectReferenceValue as Sprite) != null)
            {
                return EditorGUI.GetPropertyHeight(property, label, true) + imageHeight + 10;
            }
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            //Draw the normal property field
            EditorGUI.PropertyField(position, property, label, true);

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var sprite = property.objectReferenceValue as Sprite;

                if (sprite != null)
                {
                    position.x += EditorGUIUtility.labelWidth;
                    position.y += EditorGUI.GetPropertyHeight(property, label, true) + 5;
                    position.height = imageHeight;
                    position.width = position.height = imageHeight;

                    GUI.DrawTexture(position, sprite.texture);
                }
            }
        }
    }
}
