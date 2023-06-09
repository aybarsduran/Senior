using System;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public sealed class PlayerStaminaUI : PlayerUIBehaviour
    {
        [SerializeField]
        [Tooltip("The canvas group used to fade the stamina bar in & out.")]
        private CanvasGroup m_CanvasGroup;

        [SerializeField]
        [Tooltip("The stamina bar image, the fill amount will be modified based on the current stamina value.")]
        private Image m_StaminaBar;


        [SerializeField, Range(1f, 10f)]
        [Tooltip("Represents how much time it takes for the stamina bar to start fading after not decreasing.")]
        private float m_HideDuration = 3f;

        [SerializeField, Range(0f, 25f)]
        [Tooltip("How fast will the stamina bar alpha fade in & out.")]
        private float m_AlphaLerpSpeed = 1.5f;

        private bool m_Show;
        private float m_HideTime;
        private float m_LastStaminaValue = 1f;

        private IStaminaController m_StaminaController;


        protected override void OnAttachment()
        {
            GetModule(out m_StaminaController);
        }

        private void LateUpdate()
        {
            if (m_StaminaController == null)
                return;

            float targetAlpha = m_Show ? 1f : 0f;
            m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, targetAlpha, m_AlphaLerpSpeed * Time.deltaTime);

            if (Time.time > m_HideTime)
                m_Show = false;

            float stamina = m_StaminaController.Stamina;

            if (stamina != m_LastStaminaValue)
            {
                if (stamina < m_LastStaminaValue)
                    m_Show = true;

                m_HideTime = Time.time + m_HideDuration;
                m_StaminaBar.fillAmount = stamina;
            }

            m_LastStaminaValue = stamina;
        }
    }
}