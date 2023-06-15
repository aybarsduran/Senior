using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.Surfaces
{
    [EditorWindowTitle(title = "Surface Manager")]
    public class SurfaceManagerWindow : ToolbarEditorWindow<SurfaceManagerWindow>
    {
        protected override bool ToolbarMatchWindowWidth => true;


        [MenuItem("Tools/Surfaces", priority = 1000)]
        public static void Init()
        {
            CreateToolbarWindow("Surfaces");
        }

        protected override ToolbarEditorTab[] GetTabs()
        {
            return new ToolbarEditorTab[]
            {
                // 2: Create the build materials tab..
                new DataDefinitionsEditorTab(this, "Surfaces",
                    new DataDefinitionsEditorTab.Pair<SurfaceDefinition>("Surfaces", DataDefinitionsEditorTab.LayoutStyle.Horizontal))
                {
                    Layout = DataDefinitionsEditorTab.LayoutStyle.Vertical,
                    WindowWidthPercent = 0.95f
                }
            };
        }
    }
}