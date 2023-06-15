using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CustomEditor(typeof(ItemTagDefinition))]
    public class ItemTagDefinitionEditor : DataDefinitionBaseEditor
    {
        private static bool m_ShowFoldout = false;
        private ItemTagDefinition m_Tag;
        private List<ItemDefinition> m_ItemReferences;

        private Vector2 m_ScrollView;


        protected override void DrawCustomInspector()
        {
            serializedObject.Update();

            if (ItemManagerWindow.IsActive)
            {
                using (new GUILayout.HorizontalScope("Box"))
                {
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField((ItemTagDefinition)target, typeof(ItemTagDefinition), (ItemTagDefinition)target);
                    GUI.enabled = true;
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                DrawCustomProperty("m_TagName");

                GUI.enabled = false;
                var idProp = serializedObject.FindProperty("m_Id");
                EditorGUILayout.FloatField(idProp.intValue);
                GUI.enabled = true;
            }

            EditorGUILayout.Space();

            m_ShowFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowFoldout, "References");

            if (m_ShowFoldout)
                DrawReferences();

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawReferences()
        {
            if (m_Tag == null)
                m_Tag = target as ItemTagDefinition;

            if (m_ItemReferences == null)
                m_ItemReferences = ItemDefinition.GetAllItemsWithTag(m_Tag);

            using (var scroll = new GUILayout.ScrollViewScope(m_ScrollView, GUILayout.Height(100f)))
            {
                m_ScrollView = scroll.scrollPosition;

                GUI.enabled = false;

                if (m_ItemReferences.Count == 0)
                    GUILayout.Label("No references found..");

                foreach (var item in m_ItemReferences)
                    EditorGUILayout.ObjectField(item, typeof(ItemDefinition), false);

                GUI.enabled = true;
            }
        }
    }
}