using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CustomPropertyDrawer(typeof(ItemPropertyGenerator))]
	public class ItemPropertyGeneratorDrawer : PropertyDrawer
	{
		private ItemPropertyType m_PropType;
		private GUIContent[] m_GUIContents;
		private GUIContent[] m_Items;


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (m_GUIContents == null)
				m_GUIContents = DataDefinitionUtility.GetAllGUIContents<ItemPropertyDefinition>(true, true, false);

			if (m_Items == null)
				m_Items = DataDefinitionUtility.GetAllGUIContents<ItemDefinition>(true, true, false, new GUIContent("Empty"));

			position.height = EditorGUIUtility.singleLineHeight;

			SerializedProperty itemProp = property.FindPropertyRelative("m_ItemPropertyId");

			Rect popupRect = new Rect(position.x, position.y, position.width * 0.8f, position.height);

			int selectedProp = DataDefinitionUtility.GetIndexOfId<ItemPropertyDefinition>(itemProp.intValue); 
			selectedProp = EditorGUI.Popup(popupRect, selectedProp, m_GUIContents);

			if (selectedProp == -1)
				selectedProp = 0;

			itemProp.intValue = DataDefinitionUtility.GetIdAtIndex<ItemPropertyDefinition>(selectedProp);

			ItemPropertyDefinition prop = ItemPropertyDefinition.GetWithId(itemProp.intValue);

			if (prop == null)
				return;

			SerializedProperty valueProp = property.FindPropertyRelative("m_ValueRange");
			SerializedProperty isRandomProp = property.FindPropertyRelative("m_UseRandomValue");

			m_PropType = prop.Type;

			Rect typeRect = new Rect(position.xMax - position.width * 0.2f + EditorGUIUtility.standardVerticalSpacing, position.y, position.width * 0.2f - EditorGUIUtility.standardVerticalSpacing, position.height);
			EditorGUI.LabelField(typeRect, "Type: " + m_PropType.ToString().ToUnityLikeNameFormat(), EditorStyles.miniLabel);

			position.y = position.yMax + EditorGUIUtility.standardVerticalSpacing;

			if (m_PropType == ItemPropertyType.Boolean)
			{
				bool boolean = !Mathf.Approximately(valueProp.vector2Value.x, 0f);

				EditorGUI.LabelField(position, "True/False");
				boolean = EditorGUI.Toggle(new Rect(position.x + 86f, position.y, 16f, position.height), boolean);

				valueProp.vector2Value = new Vector2(boolean ? 1f : 0f, 0f);
			}
			else if (m_PropType == ItemPropertyType.Float || m_PropType == ItemPropertyType.Integer)
			{
				Rect selectModeRect = new Rect(position.x, position.y, position.width * 0.35f, position.height);

				int selectedMode = GUI.Toolbar(selectModeRect, isRandomProp.boolValue == true ? 1 : 0, new string[] { "Fixed", "Random" });
				isRandomProp.boolValue = selectedMode == 1;

				Rect valueRect = new Rect(selectModeRect.xMax + EditorGUIUtility.singleLineHeight, position.y, position.width - selectModeRect.width - EditorGUIUtility.singleLineHeight, position.height);

				if (selectedMode == 0)
				{
					if (m_PropType == ItemPropertyType.Float)
					{
						float value = EditorGUI.FloatField(valueRect, valueProp.vector2Value.x);
						valueProp.vector2Value = new Vector2(value, 0f);
					}
					else
					{
						float value = EditorGUI.IntField(valueRect, Mathf.RoundToInt(valueProp.vector2Value.x));
						valueProp.vector2Value = new Vector2(Mathf.Clamp(value, - 9999999, 9999999), 0);
					}
				}
				else
				{
					float[] randomRange = new float[] { valueProp.vector2Value.x, valueProp.vector2Value.y };

					if (m_PropType == ItemPropertyType.Float)
						valueProp.vector2Value = EditorGUI.Vector2Field(valueRect, GUIContent.none, valueProp.vector2Value);
					else
						valueProp.vector2Value = EditorGUI.Vector2IntField(valueRect, GUIContent.none, new Vector2Int(Mathf.Clamp(Mathf.RoundToInt(randomRange[0]), -9999999, 9999999), Mathf.Clamp(Mathf.RoundToInt(randomRange[1]), -9999999, 9999999)));
				}
			}
			else if (m_PropType == ItemPropertyType.Item)
			{
				EditorGUI.LabelField(position, "Target Item");

				Rect itemPopupRect = EditorGUI.IndentedRect(position);
				itemPopupRect = new Rect(itemPopupRect.x + 80f, itemPopupRect.y, itemPopupRect.width * 0.8f - 80f, itemPopupRect.height);

				int itemId = Mathf.RoundToInt(valueProp.vector2Value.x);

				int selectedItem = IndexOfItem(itemId);
				selectedItem = EditorGUI.Popup(itemPopupRect, selectedItem, m_Items);

				valueProp.vector2Value = new Vector2(IdOfItem(selectedItem), 0f);
			}
		}

		private int IdOfItem(int index)
		{
			if (index == 0)
				return 0;

			return DataDefinitionUtility.GetIdAtIndex<ItemDefinition>(index - 1);
		}

		private int IndexOfItem(int id)
		{
			if (id == 0)
				return 0;

			return DataDefinitionUtility.GetIndexOfId<ItemDefinition>(id) + 1;
		}
	}
}