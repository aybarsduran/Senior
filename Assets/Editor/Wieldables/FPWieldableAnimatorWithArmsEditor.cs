using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [CustomEditor(typeof(FPWieldableAnimatorWithArms))]
    public class FPWieldableAnimatorWithArmsEditor : ToolboxEditor
    {
        private static bool m_ArmsSelected = false;
        private FPWieldableAnimatorWithArms m_Animator;


        protected override void DrawCustomInspector()
        {
            serializedObject.Update();

            using (new GUILayout.VerticalScope("box"))
                DrawCustomProperty("m_Mode");

            using (new GUILayout.HorizontalScope())
            {
                if (m_Animator.Mode != FPWieldableAnimatorWithArms.AnimateMode.AnimateArmsOnly)
                {
                    if (GUILayout.Toggle(!m_ArmsSelected, "Wieldable Settings", "button"))
                        m_ArmsSelected = false;
                }
                else
                    m_ArmsSelected = true;

                if (m_Animator.Mode != FPWieldableAnimatorWithArms.AnimateMode.AnimateWieldableOnly)
                {
                    if (GUILayout.Toggle(m_ArmsSelected, "Arm Settings", "button"))
                        m_ArmsSelected = true;
                }
                else
                    m_ArmsSelected = false;
            }

            using (new GUILayout.VerticalScope("box"))
            {
                // Wieldable...
                if (!m_ArmsSelected)
                {
                    DrawCustomProperty("m_WieldableAnimator");
                    DrawCustomProperty("m_WieldableClips");
                }
                // Arms...
                else
                {
                    DrawCustomProperty("m_ArmClips");
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            m_Animator = (FPWieldableAnimatorWithArms)target;
        }
    }
}
