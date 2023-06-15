using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Wieldable), true)]
    public class WieldableEditor : FoldoutBaseTypeEditor<Wieldable>
    {
        protected override bool DrawScriptProperty => false;

        protected Wieldable wieldable;


        protected override void DrawCustomInspector()
        {
            base.DrawCustomInspector();

            if (Application.isPlaying || !Application.isEditor)
                return;

            CustomGUILayout.Separator();

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Show Model"))
                    wieldable.SetVisibility(true);

                if (GUILayout.Button("Hide Model")) 
                    wieldable.SetVisibility(false);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            wieldable = target as Wieldable;
        }
    }
}
