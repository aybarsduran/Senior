using System;
using System.Diagnostics;

namespace UnityEngine
{
    // Draws collection in form of the reorderable list.
    // <para>Supported types: any <see cref="System.Collections.IList"/>.</para>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class ReorderableListAttribute : ToolboxListPropertyAttribute
    {
        public ReorderableListAttribute(ListStyle style = ListStyle.Round, string childLabel = null, bool fixedSize = false, bool draggable = true)
        {
            Draggable = draggable;
            FixedSize = fixedSize;
            ListStyle = style;
            ChildLabel = childLabel;
        }

        public bool Draggable { get; private set; }
        public bool FixedSize { get; private set; }
        public ListStyle ListStyle { get; private set; }
        public string ChildLabel { get; private set; }

        // Indicates whether list should be allowed to fold in and out.
        public bool Foldable { get; set; }
        // Indicates whether list should have a label above elements.
        public bool HasHeader { get; set; } = true;
        // Indicates whether each element should have an additional label.
        public bool HasLabels { get; set; } = true;
    }

    public enum ListStyle
    {
        Round,
        Boxed,
        Lined
    }
}