using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    [CustomEditor(typeof(MoveToSpawnPoint))]
    public class MoveToSpawnPointEditor : ToolboxEditor
    {
        private MoveToSpawnPoint m_Target;


        protected override void DrawCustomInspector()
        {
            base.DrawCustomInspector();

            if (Application.isPlaying)
                return;

            CustomGUILayout.Separator();

            if (CustomGUILayout.ColoredButton("Move to random spawn point", CustomGUIStyles.GreenColor))
                m_Target.MoveToRandomPoint();
        }

        private void OnEnable()
        {
            m_Target = serializedObject.targetObject as MoveToSpawnPoint;
        }
    }
}