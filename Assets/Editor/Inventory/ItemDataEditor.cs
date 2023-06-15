using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CustomEditor(typeof(ItemData), editorForChildClasses: true)]
    public class ItemDataEditor : ToolboxEditor
    {
        protected override bool DrawScriptProperty => false;
    }
}