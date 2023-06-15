using IdenticalStudios.WieldableSystem;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    [EditorWindowTitle(title = "Building Manager")]
    public class BuildingManagerWindow : ToolbarEditorWindow<BuildingManagerWindow>
    {
        protected override float ToolbarWidth => 650f;


        [MenuItem("Tools/Buildings", false, priority = 1000)]
        public static void Init()
        {
            CreateToolbarWindow("Building Manager", 1300, 900, 1100, 600);
        }

        protected override ToolbarEditorTab[] GetTabs()
        {
            return new ToolbarEditorTab[]
            {
                // 1: Create the buildables and categories tab..
                new GroupDefinitionEditorTab<BuildableCategoryDefinition, BuildableDefinition>(this, "Buildables & Categories", "Categories", "Buildables")
                {
                    GroupListWidthPercent = 0.2f,
                    MembersListWidthPercent = 0.25f,
                    MemberInspectorWidthPercent = 0.5f
                },

                // 2: Create the build materials tab..
                new DataDefinitionsEditorTab(this, "Build Materials",
                    new DataDefinitionsEditorTab.Pair<BuildMaterialDefinition>("Build Materials", DataDefinitionsEditorTab.LayoutStyle.Horizontal))
                {
                    Layout = DataDefinitionsEditorTab.LayoutStyle.Vertical, 
                    WindowWidthPercent = 0.95f
                },

                // (TODO: move to different window)
                // 3: Create the carriables tab..
                new DataDefinitionsEditorTab(this, "Carriables",
                    new DataDefinitionsEditorTab.Pair<CarriableDefinition>("Carriables", DataDefinitionsEditorTab.LayoutStyle.Horizontal))
                {
                    Layout = DataDefinitionsEditorTab.LayoutStyle.Vertical,
                    WindowWidthPercent = 0.95f
                },

                // 4: Create the settings tab..
                new GenericEditorTab(this, "Building Settings", Resources.LoadAll<BuildingManager>("")[0])
            };
        }
    }
}