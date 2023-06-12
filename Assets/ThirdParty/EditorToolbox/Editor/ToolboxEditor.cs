﻿using System;
using System.Collections.Generic;
using UnityEditor;

namespace Toolbox.Editor
{
    using Editor = UnityEditor.Editor;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Base Editor class for all Toolbox-related features.
    /// </summary>
    [CustomEditor(typeof(Object), true, isFallback = true)]
    [CanEditMultipleObjects]
    public class ToolboxEditor : Editor
    {
        protected virtual bool DrawScriptProperty => true;

        private readonly HashSet<string> propertiesToIgnore = new HashSet<string>();


        /// <summary>
        /// Inspector GUI re-draw call.
        /// </summary>
        public sealed override void OnInspectorGUI()
        {
            try
            {
                OnBeginToolboxEditor?.Invoke(this);
                DrawCustomInspector();
            }
            catch (Exception)
            {
                //make sure to catch all Exceptions (especially ExitGUIException),
                //it will allow us to safely dispose all layout-based controls, etc.
                OnBreakToolboxEditor?.Invoke(this);
                throw;
            }
            finally
            {
                OnCloseToolboxEditor?.Invoke(this);
            }
        }

        protected void DrawCustomProperty(string propertyPath)
        {
            if (propertiesToIgnore.Contains(propertyPath))
                return;

            var property = serializedObject.FindProperty(propertyPath);
            ToolboxEditorGui.DrawToolboxProperty(property);
        }

        protected void DrawCustomPropertySkipIgnore(string propertyPath)
        {
            var property = serializedObject.FindProperty(propertyPath);
            ToolboxEditorGui.DrawToolboxProperty(property);
        }

        /// <summary>
        /// Handles property display process using custom <see cref="Drawers.ToolboxDrawer"/>.
        /// </summary>
        /// <param name="property">Property to display.</param>
        protected void DrawCustomProperty(SerializedProperty property)
        {
            var propertyPath = property.propertyPath;
            if (propertiesToIgnore.Contains(propertyPath))
                return;

            ToolboxEditorGui.DrawToolboxProperty(property);
        }

        /// <summary>
        /// Draws each available property using internal <see cref="Drawers.ToolboxDrawer"/>s.
        /// </summary>
        protected virtual void DrawCustomInspector()
        {
            if (ToolboxDrawerModule.ToolboxDrawersAllowed)
            {
                serializedObject.Update();
                
                var property = serializedObject.GetIterator();
                
                //enter to the 'Base' property
                if (property.NextVisible(true))
                {
                    if (DrawScriptProperty)
                    {
                        var script = PropertyUtility.IsDefaultScriptProperty(property);

                        //try to draw the first property (m_Script)
                        using (new EditorGUI.DisabledScope(script))
                        {
                            DrawCustomProperty(property.Copy());
                        }
                    }

                    //iterate over all other serialized properties
                    //NOTE: every child will be handled internally
                    while (property.NextVisible(false))
                    {
                        DrawCustomProperty(property.Copy());
                    }
                }

                serializedObject.ApplyModifiedProperties();
                return;
            }

            DrawDefaultInspector();
        }

        /// <summary>
        /// Forces provided <see cref="SerializedProperty"/> to be ignored in the drawing process.
        /// </summary>
        protected void IgnoreProperty(SerializedProperty property) => IgnoreProperty(property.propertyPath);

        /// <summary>
        /// Forces associated <see cref="SerializedProperty"/> to be ignored in the drawing process.
        /// </summary>
        public void IgnoreProperty(string propertyPath) => propertiesToIgnore.Add(propertyPath);

        public static event Action<Editor> OnBeginToolboxEditor;
        public static event Action<Editor> OnBreakToolboxEditor;
        public static event Action<Editor> OnCloseToolboxEditor;
    }
}