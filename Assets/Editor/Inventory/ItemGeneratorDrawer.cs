using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CustomPropertyDrawer(typeof(ItemGenerator))]
    public class ItemGeneratorDrawer : PropertyDrawer
    {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var method = property.FindPropertyRelative("m_Method");
			var category = property.FindPropertyRelative("m_Category");
			var item = property.FindPropertyRelative("m_SpecificItem");
			var minCount = property.FindPropertyRelative("m_MinCount");
			var maxCount = property.FindPropertyRelative("m_MaxCount");

			position.x -= 4f;
			float spacing = 4f;

			EditorGUI.indentLevel -= 1;

			// Method
			position.height = 16f;
			position.y += spacing;
			position.x += 16f;
			position.width -= 16f;
			EditorGUI.PropertyField(position, method);

			ItemGenerationMethod parsedMethod = (ItemGenerationMethod)method.enumValueIndex;

			if (parsedMethod == ItemGenerationMethod.RandomFromCategory)
			{
				// Category
				position.y = position.yMax + spacing;
				EditorGUI.PropertyField(position, category);
			}
			else if (parsedMethod == ItemGenerationMethod.Specific)
			{
				// Item
				position.y = position.yMax + spacing;
				EditorGUI.PropertyField(position, item);
			}

			// Min Count
			position.y = position.yMax + spacing;
			EditorGUI.PropertyField(position, minCount);

			// Max Count
			position.y = position.yMax + spacing;
			EditorGUI.PropertyField(position, maxCount);

			EditorGUI.indentLevel += 1;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			ItemGenerationMethod method = (ItemGenerationMethod)property.FindPropertyRelative("m_Method").enumValueIndex;

			float defaultHeight = 10f;
			float height = 40;
			float spacing = 4f;

			if (method == ItemGenerationMethod.Random)
				height += (defaultHeight + spacing) * 2;
			else
				height += (defaultHeight + spacing) * 3;

			return height;
		}
	}
}