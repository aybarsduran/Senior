using Toolbox.Editor;
using Toolbox.Editor.Drawers;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CustomPropertyDrawer(typeof(DataIdReference<>))]
    [CustomPropertyDrawer(typeof(DataNameReference<>))]
    [CustomPropertyDrawer(typeof(DataReferenceDetailsAttribute))]
    public class DataReferenceDrawer : PropertyDrawerBase
    {
        private IDataReferenceHandler m_DataReference;
        private IDataReferenceHandler DataReference
        {
            get
            {
                if (m_DataReference == null)
                    m_DataReference = m_Property.GetValue<IDataReferenceHandler>();

                return m_DataReference;
            }
        }

        private DataReferenceDetailsAttribute n_DataDetails;
        private DataReferenceDetailsAttribute DataDetails
        {
            get
            {
                if (n_DataDetails == null)
                    n_DataDetails = PropertyUtility.GetAttribute<DataReferenceDetailsAttribute>(m_Property);

                return n_DataDetails;
            }
        }

        private SerializedProperty m_Property;
        private GUIContent[] m_Contents;
        private int m_LastDataCount;


        public override bool IsPropertyValid(SerializedProperty property) => !property.hasMultipleDifferentValues;

        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            m_Property = property;
            float extraHeight = (DataDetails != null && DataDetails.HasAssetReference) ? EditorGUIUtility.singleLineHeight + 3f : 0f;
            return base.GetPropertyHeightSafe(property, label) + extraHeight;
        }

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            m_Property = property;
            var valueProperty = property.FindPropertyRelative("m_Value");

            if (DataReference.GetDataCount() != m_LastDataCount)
                Initialize(valueProperty);

            var popupRect = new Rect(position) { height = EditorGUIUtility.singleLineHeight };
            label.text = HasLabel() ? label.text : string.Empty;

            int selectedIndex = 0;

            switch (valueProperty.propertyType)
            {
                case SerializedPropertyType.Integer:
                    selectedIndex = IndexOfElement(valueProperty.intValue);
                    selectedIndex = EditorGUI.Popup(popupRect, label, selectedIndex, m_Contents);
                    valueProperty.intValue = IdOfElement(selectedIndex);
                    break;
                case SerializedPropertyType.String:
                    selectedIndex = IndexOfContent(m_Contents, valueProperty.stringValue);
                    selectedIndex = EditorGUI.Popup(position, label, selectedIndex, m_Contents);
                    valueProperty.stringValue = ContentAtIndex(m_Contents, selectedIndex).text;
                    break;
            }

            if (HasAssetReference())
                DrawAssetReference(position, selectedIndex);
        }

        private void Initialize(SerializedProperty valueProperty)
        {
            // Sets initial id
            if (valueProperty.propertyType == SerializedPropertyType.Integer)
                valueProperty.intValue = IsValidId(valueProperty.intValue) ? IdOfElement(0) : valueProperty.intValue;

            m_Contents = DataReference.GetAllGUIContents(true, true, HasIcon(), HasNullElement() ? (DataDetails == null ? new GUIContent("Empty") : new GUIContent(DataDetails.NullElementName)) : null);
            m_LastDataCount = DataReference.GetDataCount();
        }

        private void DrawAssetReference(Rect position, int selectedIndex)
        {
            if (HasNullElement())
                selectedIndex -= 1;

            GUI.enabled = false;
            float lineHeight = EditorGUIUtility.singleLineHeight;
            position = new Rect(position.x + lineHeight / 2, position.y + lineHeight, position.width - lineHeight, lineHeight);

            var scriptable = DataReference.GetDataAtIndex(selectedIndex);
            EditorGUI.ObjectField(position, "Asset", scriptable, typeof(ScriptableObject), false);
            GUI.enabled = true;
        }

        private static bool IsValidId(int id)
        {
            bool isValid = id <= -1 && id >= 1;
            return isValid;
        }

        private int IdOfElement(int index)
        {
            if (HasNullElement() && index == 0)
                return 0;
            
            return DataReference.GetIdAtIndex(index - (HasNullElement() ? 1 : 0));
        }

        private int IndexOfElement(int id)
        {
            if (HasNullElement() && id == 0)
                return 0;
            
            return DataReference.GetIndexOfId(id) + (HasNullElement() ? 1 : 0);
        }

        private int IndexOfContent(GUIContent[] allStrings, string str)
        {
            for (int i = 0; i < allStrings.Length; i++)
            {
                if (allStrings[i].text == str)
                    return i;
            }

            return 0;
        }

        private GUIContent ContentAtIndex(GUIContent[] allStrings, int i)
        {
            return allStrings.Length > i ? allStrings[i] : GUIContent.none;
        }

        private bool HasLabel()
        {
            bool drawLabel = DataDetails == null || DataDetails.HasLabel;
            return drawLabel;
        }

        private bool HasNullElement()
        {
            bool hasNullElement = DataDetails == null || DataDetails.HasNullElement;
            return hasNullElement;
        }

        private bool HasIcon()
        {
            bool hasIcon = DataDetails == null || DataDetails.HasIcon;
            return hasIcon;
        }

        private bool HasAssetReference()
        {
            bool drawLabel = DataDetails != null && DataDetails.HasAssetReference;
            return drawLabel;
        }
    }
}