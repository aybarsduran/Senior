using UnityEditor;

namespace IdenticalStudios.InventorySystem
{
    [EditorWindowTitle(title = "Item Manager")]
    public class ItemManagerWindow : ToolbarEditorWindow<ItemManagerWindow>
    {
        [MenuItem("Tools/Items", priority = 1000)]
        public static void Init()
        {
            CreateToolbarWindow("Item Manager");
        }

        protected override ToolbarEditorTab[] GetTabs()
        {
            return new ToolbarEditorTab[]
            {
                // 1: Create the items and categories tab..
                new GroupDefinitionEditorTab<ItemCategoryDefinition, ItemDefinition>(this, "Items & Categories", "Categories", "Items"),

                // 2: Create the properties and tags tab..
                new DataDefinitionsEditorTab(this, "Properties & Tags",
                    new DataDefinitionsEditorTab.Pair<ItemPropertyDefinition>("Properties"),
                    new DataDefinitionsEditorTab.Pair<ItemTagDefinition>("Tags"))
                {
                    Layout = DataDefinitionsEditorTab.LayoutStyle.Horizontal
                },

                // 3: Create the the settings tab..
                new GenericEditorTab(this, "Tools", CreateInstance<DataDefinitionTools>())
            };
        }
    }
}
