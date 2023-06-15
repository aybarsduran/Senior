using UnityEditor;
using UnityEngine;
using Toolbox.Editor;

namespace IdenticalStudios
{
    [CustomEditor(typeof(Character), true)]
    public class CharacterEditor : ToolboxEditor
    {
        private ICharacterModule[] m_Modules;
        private bool m_FoldoutActive = false; 

        private static Color m_GUIColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        private static GUIStyle m_GUIStyle;
        private const string m_ModulesTxt = " Modules";


        protected override void DrawCustomInspector()
        {
            base.DrawCustomInspector();

            if (m_Modules != null && m_Modules.Length > 0 && !Application.isPlaying && Application.isEditor)
            {
                if (m_GUIStyle == null && CustomGUIStyles.BoldMiniGreyLabel != null)
                {
                    m_GUIStyle = new GUIStyle(CustomGUIStyles.BoldMiniGreyLabel);
                    m_GUIStyle.fontSize = 12;
                }

                EditorGUILayout.Space();

                int index = 1;

                m_FoldoutActive = EditorGUILayout.Foldout(m_FoldoutActive, m_FoldoutActive ? m_ModulesTxt : m_ModulesTxt + "...", true, CustomGUIStyles.StandardFoldout);

                if (m_FoldoutActive)
                {
                    CustomGUILayout.Separator();

                    GUI.color = m_GUIColor;

                    foreach (var module in m_Modules)
                    {
                        GUILayout.Label($" {index}: {module.GetType().Name}".ToUnityLikeNameFormat(), m_GUIStyle);
                        index++;
                    }
                }
            }
        }
         
        private void OnEnable()
        {
            var character = target as Character;

            if (character != null)
                m_Modules = character.GetComponentsInChildren<ICharacterModule>();
        }
    }
}