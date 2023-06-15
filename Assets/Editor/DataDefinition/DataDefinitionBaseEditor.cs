using Toolbox.Editor;
using UnityEditor;

namespace IdenticalStudios
{
    [CustomEditor(typeof(DataDefinitionBase), true)]
    public class DataDefinitionBaseEditor : ToolboxEditor
    {
        protected override bool DrawScriptProperty => !ToolbarEditorWindowBase.HasActiveWindows;
    }
}
