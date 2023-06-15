using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace IdenticalStudios
{
    [CustomPropertyDrawer(typeof(AnimatorParameterTrigger))]
	public class AnimatorParameterTriggerDrawer : PropertyDrawer
	{
        private SerializedProperty m_TypeProp;
        private SerializedProperty m_NameProp;
        private SerializedProperty m_ValueProp;

        private const float k_Indentation = 6f;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
            if (Application.isPlaying)
                return;

            float separatorX = position.x - k_Indentation;
            CustomGUILayout.Separator(new Rect(separatorX, position.y - 1f, position.width + k_Indentation / 2, 1f));

            position.height = EditorGUIUtility.singleLineHeight;
			property.isExpanded = true;
            
            m_TypeProp = property.FindPropertyRelative("m_Type");
            m_NameProp = property.FindPropertyRelative("m_Name");
            m_ValueProp = property.FindPropertyRelative("m_Value");

            EditorGUI.PropertyField(position, m_TypeProp, true); 

            var paramType = m_TypeProp.GetValue<AnimatorControllerParameterType>();
            position.y = position.yMax + EditorGUIUtility.standardVerticalSpacing;

            // Draw animator param
            if (paramType != m_AnimatorParameterType)
                m_SelectedValue = 0;

            m_AnimatorParameterType = paramType;
            AnimatorParameters(position, m_NameProp, label);

            if (paramType == AnimatorControllerParameterType.Bool)
            {
                position = new Rect(position.x + k_Indentation, position.yMax + EditorGUIUtility.standardVerticalSpacing, position.width - k_Indentation, position.height);

                bool boolean = !Mathf.Approximately(m_ValueProp.floatValue, 0f);

                boolean = EditorGUI.Toggle(position, "Bool: ", boolean);

                m_ValueProp.floatValue = boolean ? 1f : 0f;
            }
            else if (paramType == AnimatorControllerParameterType.Float || paramType == AnimatorControllerParameterType.Int)
            {
                position = new Rect(position.x + k_Indentation, position.yMax + EditorGUIUtility.standardVerticalSpacing, position.width - k_Indentation, position.height);

                if (paramType == AnimatorControllerParameterType.Float)
                    m_ValueProp.floatValue = EditorGUI.FloatField(position, "Float: ", m_ValueProp.floatValue);
                else
                    m_ValueProp.floatValue = Mathf.Clamp(EditorGUI.IntField(position, "Integer: ", Mathf.RoundToInt(m_ValueProp.floatValue)), -9999999, 9999999);
            }
            else
            {
                position = new Rect(position.x + k_Indentation, position.yMax + EditorGUIUtility.standardVerticalSpacing, position.width - k_Indentation, position.height);
                EditorGUI.LabelField(position, "Trigger: ");
            }

            CustomGUILayout.Separator(new Rect(separatorX, position.yMax + EditorGUIUtility.standardVerticalSpacing - 1f, position.width + k_Indentation / 2, 1f));
        }

		public override bool CanCacheInspectorGUI(SerializedProperty property) => false;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
            if (Application.isPlaying)
                return 4f;

            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return height * 3f;
        }

        #region Hack

        private AnimatorControllerParameterType m_AnimatorParameterType;
        private int m_SelectedValue;


        private void AnimatorParameters(Rect position, SerializedProperty property, GUIContent label)
        {
            var animatorController = GetAnimatorController(property);

            if (animatorController == null)
            {
                DefaultInspector(position, property, label);
                return;
            }

            var parameters = animatorController.parameters;

            if (parameters.Length == 0)
            {
                Debug.LogWarning("AnimationParamater is 0");
                property.stringValue = string.Empty;
                DefaultInspector(position, property, label);
                return;
            }

            var eventNames = parameters
                .Where(t => CanAddEventName(t.type))
                .Select(t => t.name).ToList();

            if (eventNames.Count == 0)
            {
                Debug.LogWarning(string.Format("{0} Parameter is 0", m_AnimatorParameterType));
                property.stringValue = string.Empty;
                DefaultInspector(position, property, label);
                return;
            }

            var eventNamesArray = eventNames.ToArray();

            var matchIndex = eventNames
                .FindIndex(eventName => eventName.Equals(property.stringValue));

            if (matchIndex != -1)
                m_SelectedValue = matchIndex;

            m_SelectedValue = EditorGUI.IntPopup(position, "Param: ", m_SelectedValue, eventNamesArray, SetOptionValues(eventNamesArray));

            property.stringValue = eventNamesArray[m_SelectedValue];
        }

        private AnimatorController GetAnimatorController(SerializedProperty property)
        {
            if (property.serializedObject.targetObject is not Component component)
                return null;

            // Try get animator in children
            var anim = component.GetComponentInChildren<Animator>();

            // Try get animator in sibling
            if (anim == null && component.transform.parent != null)
                anim = component.transform.parent.GetComponentInChildren<Animator>();

            // Try get animator in parent
            if (anim == null)
                anim = component.GetComponentInParent<Animator>();

            if (anim == null)
            {
                Debug.LogException(new MissingComponentException("Missing Animator Component"));
                return null;
            }

            return anim.runtimeAnimatorController as AnimatorController;
        }

        private bool CanAddEventName(AnimatorControllerParameterType animatorControllerParameterType)
        {
            return (int)animatorControllerParameterType == (int)m_AnimatorParameterType;
        }

        private int[] SetOptionValues(string[] eventNames)
        {
            int[] optionValues = new int[eventNames.Length];
            for (int i = 0; i < eventNames.Length; i++)
            {
                optionValues[i] = i;
            }
            return optionValues;
        }

        private void DefaultInspector(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label);
        }
        #endregion
    }
}