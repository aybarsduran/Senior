using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace IdenticalStudios
{
    [CustomPropertyDrawer(typeof(AnimationOverrideClips))]
    public class OverrideAnimationClipsDrawer : PropertyDrawer
    {
        private bool m_Initialized;
        private SerializedProperty m_Controller;
        private SerializedProperty m_Clips;

        private ReorderableList m_ReordClipList;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!m_Initialized)
                Initialize(property);

            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, m_Controller);

            RuntimeAnimatorController controller = m_Controller.objectReferenceValue as RuntimeAnimatorController;

            if (EditorGUI.EndChangeCheck())
            {
                if(m_Controller.objectReferenceValue == null)
                    m_Clips.arraySize = 0;
                else
                    GetClipsFromController(controller);
            }

            if (controller != null && controller.animationClips.Length != m_Clips.arraySize)
                GetClipsFromController(controller);

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            m_ReordClipList.DoList(EditorGUI.IndentedRect(position));

            position.y += m_ReordClipList.GetHeight();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!m_Initialized)
                Initialize(property);

            return m_ReordClipList.GetHeight() + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        private void DrawClipElement(Rect rect, int index, bool selected, bool focused)
        {
            AnimationClip originalClip = m_Clips.GetArrayElementAtIndex(index).FindPropertyRelative("Original").objectReferenceValue as AnimationClip;
            AnimationClip overrideClip = m_Clips.GetArrayElementAtIndex(index).FindPropertyRelative("Override").objectReferenceValue as AnimationClip;

            rect.xMax = rect.xMax / 2.0f;
            GUI.Label(rect, originalClip.name, EditorStyles.label);
            rect.xMin = rect.xMax;
            rect.xMax *= 2.0f;

            EditorGUI.BeginChangeCheck();
            overrideClip = EditorGUI.ObjectField(rect, "", overrideClip, typeof(AnimationClip), false) as AnimationClip;

            if (EditorGUI.EndChangeCheck())
                m_Clips.GetArrayElementAtIndex(index).FindPropertyRelative("Override").objectReferenceValue = overrideClip;
        }

        private void SelectClip(ReorderableList list)
        {
            if (0 <= list.index && list.index < m_Clips.arraySize)
                EditorGUIUtility.PingObject(m_Clips.GetArrayElementAtIndex(list.index).FindPropertyRelative("Original").objectReferenceValue);
        }

        private void DrawClipHeader(Rect rect)
        {
            rect.xMax = rect.xMax / 2.0f;
            GUI.Label(rect, "Original", EditorStyles.label);

            rect.xMin = rect.xMax + 14;
            rect.xMax *= 2.0f;
            GUI.Label(rect, "Override", EditorStyles.label);
        }

        private void GetClipsFromController(RuntimeAnimatorController controller)
        {
            var clips = controller.animationClips;

            m_Clips.arraySize = clips.Length;

            int i = 0;

            foreach(SerializedProperty clipPair in m_Clips)
            {
                clipPair.FindPropertyRelative("Original").objectReferenceValue = clips[i];
                i++;
            }
        }

        private void Initialize(SerializedProperty property)
        {
            m_Controller = property.FindPropertyRelative("m_Controller");
            m_Clips = property.FindPropertyRelative("m_Clips");

            m_ReordClipList = new ReorderableList(property.serializedObject, m_Clips);
            m_ReordClipList.draggable = false;
            m_ReordClipList.displayAdd = m_ReordClipList.displayRemove = false;
            m_ReordClipList.drawElementCallback = DrawClipElement;
            m_ReordClipList.drawHeaderCallback = DrawClipHeader;
            m_ReordClipList.drawNoneElementCallback = DrawNoneElementCallback;
            m_ReordClipList.elementHeight = EditorGUIUtility.singleLineHeight;
            m_ReordClipList.onSelectCallback = SelectClip;
            m_ReordClipList.footerHeight = 0f;

            m_Initialized = true;
            
            void DrawNoneElementCallback(Rect rect)
            {
                GUI.Label(rect, m_Controller.objectReferenceValue == null
                    ? "Assign an animator controller"
                    : "The animator controller has no clips");
            }
        }
    }
}