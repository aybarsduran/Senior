using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    [CanEditMultipleObjects]
	[CustomEditor(typeof(PanelUI), true)]
	public class PanelUIEditor : ToolboxEditor
	{
		protected override void DrawCustomInspector()
		{
			base.DrawCustomInspector();

			EditorGUILayout.Space();

			using (new GUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Show"))
				{
					foreach (var target in serializedObject.targetObjects)
						ShowPanel(target as PanelUI);
				}

				if (GUILayout.Button("Hide"))
				{
					foreach (var target in serializedObject.targetObjects)
						HidePanel(target as PanelUI);
				}
			}
		}

		private void ShowPanel(PanelUI panel)
		{
			var canvasGroup = panel.CanvasGroup;
			canvasGroup.alpha = 1f;
			canvasGroup.blocksRaycasts = true;
			canvasGroup.interactable = true;
			EditorUtility.SetDirty(canvasGroup);
		}

		private void HidePanel(PanelUI panel)
		{
			var canvasGroup = panel.CanvasGroup;
			canvasGroup.alpha = 0f;
			canvasGroup.blocksRaycasts = false;
			canvasGroup.interactable = false;
			EditorUtility.SetDirty(canvasGroup);
		}
	}
}