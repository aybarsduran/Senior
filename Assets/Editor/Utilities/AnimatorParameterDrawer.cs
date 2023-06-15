using System.Linq;
using Toolbox.Editor.Drawers;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using System;
#endif

namespace IdenticalStudios
{
    [CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
    public class AnimatorParameterDrawer : PropertyDrawerBase
    {
        private AnimatorController m_Animator;


        public override bool IsPropertyValid(SerializedProperty property)
        {
            m_Animator = GetAnimatorController(property);
            return m_Animator != null;
        }
        
        public override bool CanCacheInspectorGUI(SerializedProperty property) => false;

        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            var parameters = m_Animator.parameters;

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
                Debug.LogWarning(string.Format("{0} Parameter is 0", AnimatorParameterAttribute.ParameterType));
                property.stringValue = string.Empty;
                DefaultInspector(position, property, label);
                return;
            }

            var eventNamesArray = eventNames.ToArray();

            var matchIndex = eventNames
                .FindIndex(eventName => eventName.Equals(property.stringValue));

            if (matchIndex != -1)
                AnimatorParameterAttribute.SelectedValue = matchIndex;

            AnimatorParameterAttribute.SelectedValue = EditorGUI.IntPopup(position, label.text, AnimatorParameterAttribute.SelectedValue, eventNamesArray, SetOptionValues(eventNamesArray));

            property.stringValue = eventNamesArray[AnimatorParameterAttribute.SelectedValue];
        }
        
        private AnimatorController GetAnimatorController(SerializedProperty property)
        {
            var component = property.serializedObject.targetObject as Component;

            if (component == null)
                throw new InvalidCastException("Couldn't cast targetObject");

            // Try get animator in children
            var anim = component.GetComponentInChildren<Animator>();

            // Try get animator in parent
            if (anim == null)
                anim = component.GetComponentInParent<Animator>();

            return anim != null ? anim.runtimeAnimatorController as AnimatorController : null;
        }

        private bool CanAddEventName(AnimatorControllerParameterType animatorControllerParameterType)
        {
            return (int)animatorControllerParameterType == (int)AnimatorParameterAttribute.ParameterType;
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

        private AnimatorParameterAttribute AnimatorParameterAttribute => (AnimatorParameterAttribute)attribute;

        private void DefaultInspector(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}