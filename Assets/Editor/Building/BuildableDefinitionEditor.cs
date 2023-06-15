using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    [CustomEditor(typeof(BuildableDefinition))]
    public class BuildableDefinitionEditor : ToolboxEditor
    {
        private BuildableDefinition m_Buildable;
        private GUIStyle BoxStyle => new GUIStyle("Box");
        private Rect m_CreatePickupPopupRect;


        protected override void DrawCustomInspector()
        {
            if (serializedObject == null)
                return;

            GUI.color = Color.white;
            serializedObject.Update();

            DrawBuildableName();
            DrawNonInteractableInfo();
            DrawBuildableInfo();
            DrawBuildRequirements();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            m_Buildable = target as BuildableDefinition;
        }

        #region Buildable Info
        private void DrawBuildableName()
        {
            using (new GUILayout.VerticalScope(BoxStyle))
            {
                DrawCustomProperty("m_BuildableName");
            }
        }

        private void DrawNonInteractableInfo()
        {
            using (new GUILayout.VerticalScope(BoxStyle))
            {
                DrawCustomProperty("m_Id");

                GUI.enabled = false;
                EditorGUILayout.ObjectField("Asset", m_Buildable, typeof(BuildableDefinition), false);
                GUI.enabled = true;

                DrawCustomProperty("m_ParentGroup");

                EditorGUILayout.Space();
            }
        }

        private void DrawBuildableInfo()
        {
            using (new CustomGUILayout.VerticalScope(BoxStyle))
            {
                //Draw icon property..
                if (serializedObject.FindProperty("m_Icon").objectReferenceValue == null)
                {
                    using (new CustomGUILayout.HorizontalScope(BoxStyle))
                    {
                        DrawCustomProperty("m_Icon");

                        if (GUILayout.Button("Find Matching Icon"))
                            FindMatchingIcon();
                    }
                }
                else
                    DrawCustomProperty("m_Icon");

                // Draw pickup property..
                if (serializedObject.FindProperty("m_Prefab").objectReferenceValue == null)
                {
                    using (new GUILayout.HorizontalScope(BoxStyle))
                    {
                        DrawCustomProperty("m_Prefab");

                        if (CustomGUILayout.ColoredButton("Find Matching Prefab", CustomGUIStyles.YellowColor))
                            FindMatchingPrefab();

                        if (CustomGUILayout.ColoredButton("Create Prefab", CustomGUIStyles.GreenColor))
                            CreateBuildablePrefab();

                        if (Event.current.type == EventType.Repaint) m_CreatePickupPopupRect = GUILayoutUtility.GetLastRect();
                    }
                }
                else
                    DrawCustomProperty("m_Prefab");

                // Draw description property..
                DrawCustomProperty("m_Description");
            }
        }

        private void FindMatchingIcon()
        {
            var matchingSprite = AssetDatabaseHelper.FindClosestMatchingObjectWithName<Sprite>(GetCorrectBuildableName());

            if (matchingSprite != null)
                serializedObject.FindProperty("m_Icon").SetValue(matchingSprite);
        }

        private void FindMatchingPrefab()
        {
            var matchingPickup = AssetDatabaseHelper.FindClosestMatchingPrefab<Buildable>(GetCorrectBuildableName(), "(Buildable)");

            if (matchingPickup != null)
                serializedObject.FindProperty("m_Prefab").SetValue(matchingPickup);
        }

        private void CreateBuildablePrefab()
        {
            PopupWindow.Show(m_CreatePickupPopupRect, new CreateBuildablePopup(m_Buildable, AssignPrefab));
        }

        private void AssignPrefab(GameObject prefab)
        {
            serializedObject.FindProperty("m_Prefab").SetValue(prefab.GetComponent<Buildable>());
        }

        private string GetCorrectBuildableName()
        {
            if (string.IsNullOrEmpty(m_Buildable.Name))
                return m_Buildable.name;

            return m_Buildable.Name;
        }

        private void DrawBuildRequirements()
        {
            using (new GUILayout.VerticalScope(BoxStyle))
            {
                DrawCustomProperty("m_BuildRequirements");
            }
        }
        #endregion
    }
}
