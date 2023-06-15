using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CustomEditor(typeof(ItemPropertyDefinition))]
    public class ItemPropertyDefinitionEditor : DataDefinitionBaseEditor
    {
        private static bool m_ShowFoldout = false;
        private ItemPropertyDefinition m_Property;
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
                    EditorGUILayout.ObjectField((ItemPropertyDefinition)target, typeof(ItemPropertyDefinition), (ItemPropertyDefinition)target);
                    GUI.enabled = true;
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                DrawCustomProperty("m_PropertyName");

                GUI.enabled = false;
                var idProp = serializedObject.FindProperty("m_Id");
                EditorGUILayout.FloatField(idProp.intValue);
                GUI.enabled = true;

                DrawCustomProperty("m_PropertyType");
            }

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

            GUI.color = new Color(0.85f, 0.85f, 0.85f, 0.95f);
            DrawCustomProperty("m_Description");
            GUI.color = Color.white;

            EditorGUILayout.Space();

            m_ShowFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(m_ShowFoldout, "References");

            if (m_ShowFoldout)
                DrawReferences();

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawReferences()
        {
            if (m_Property == null)
                m_Property = target as ItemPropertyDefinition;

            if (m_ItemReferences == null)
                m_ItemReferences = ItemDefinition.GetAllItemsWithProperty(m_Property);

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