using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    [CustomEditor(typeof(WorkstationInspectControllerUI))]
    public class WorkstationInspectControllerUIEditor : ToolboxEditor
    {
        protected override void DrawCustomInspector()
        {
            base.DrawCustomInspector();

            CustomGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Show All Inspectable Panels"))
            {
                ShowAllChildPanels(true);
                return;
            }

            if (GUILayout.Button("Hide All Inspectable Panels"))
                ShowAllChildPanels(false);

            EditorGUILayout.EndHorizontal();
        }

        private void ShowAllChildPanels(bool show)
        {
            var obj = (target as Component).gameObject;

            if (obj == null)
                return;

            var panels = obj.GetComponentsInChildren<PanelUI>();

            foreach (var panel in panels)
            {
                panel.GetComponentInChildren<CanvasGroup>().alpha = show ? 1f : 0f;
                panel.GetComponentInChildren<CanvasGroup>().blocksRaycasts = show;
            }
        }
    }
}