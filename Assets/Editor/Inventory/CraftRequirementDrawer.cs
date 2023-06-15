using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CustomPropertyDrawer(typeof(CraftRequirement))]
	public class CraftRequirementDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty itemNameProp = property.FindPropertyRelative("Item");
			EditorGUI.MultiPropertyField(EditorGUI.IndentedRect(position), new GUIContent[] { new GUIContent(""), new GUIContent("") }, itemNameProp);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;
	}
}