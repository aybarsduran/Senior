using IdenticalStudios.UISystem;
using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    [CanEditMultipleObjects]
	[CustomEditor(typeof(ItemContainerUI), true)]
	public class ItemContainerUIEditor : ToolboxEditor
	{
		private SerializedProperty m_SlotTemplate;
		private SerializedProperty m_SlotsParent;
		private int m_SlotCount = 0;


		protected override void DrawCustomInspector()
        {
			base.DrawCustomInspector();

			serializedObject.Update();

			EditorGUILayout.Space();

			if (!Application.isPlaying)
			{
				CustomGUILayout.Separator();
				using (new GUILayout.HorizontalScope())
				{
					if (!serializedObject.isEditingMultipleObjects && CustomGUILayout.ColoredButton("Spawn Default Slots", CustomGUIStyles.GreenColor))
						(serializedObject.targetObject as ItemContainerUI).GenerateSlots(m_SlotCount);

					m_SlotCount = EditorGUILayout.IntField(m_SlotCount);
					m_SlotCount = Mathf.Clamp(m_SlotCount, 0, 100);
				}
			}

			if (!m_SlotTemplate.objectReferenceValue || !m_SlotsParent.objectReferenceValue)
				EditorGUILayout.HelpBox("Make sure a slot template and parent are assigned!", MessageType.Error);

			serializedObject.ApplyModifiedProperties();
		}

		private void OnEnable()
		{
			m_SlotTemplate = serializedObject.FindProperty("m_SlotTemplate");
			m_SlotsParent = serializedObject.FindProperty("m_SlotsRoot");

			if (m_SlotsParent != null)
			{
				foreach (Transform child in (Transform)m_SlotsParent.objectReferenceValue)
				{
					if (child.gameObject.HasComponent(typeof(ItemSlotUI)))
						m_SlotCount++;
				}
			}
		}
	}
}
