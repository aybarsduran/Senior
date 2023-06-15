using UnityEngine;
using UnityEditor;
using Toolbox.Editor;

namespace IdenticalStudios.Surfaces
{
    [CustomEditor(typeof(SurfaceManager))]
    public class SurfaceManagerEditor : ToolboxEditor
    {
        protected override void DrawCustomInspector()
        {
            base.DrawCustomInspector();

            EditorGUILayout.Space();

            if (GUILayout.Button("Open Surface Editor", EditorStyles.miniButtonMid))
                SurfaceManagerWindow.Init();
        }
    }
}