using IdenticalStudios.InputSystem;
using IdenticalStudios.WieldableSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public sealed class CrosshairManagerUI : PlayerUIBehaviour
    {
        public static bool CrosshairEnabled { get; set; } = true;

        [SerializeField]
        [Tooltip("Default Crosshair: The default (starting) crosshair.")]
        private CrosshairBaseUI m_DefaultCrosshair;

        [SpaceArea]

        [SerializeField]
        [Tooltip("The canvas group used to fade a crosshair in & out.")]
        private CanvasGroup m_CanvasGroup;

        [SerializeField, Range(0.1f, 1f)]
        [Tooltip("The max crosshair alpha.")]
        private float m_CrosshairAlpha = 0.7f;

        [SerializeField, Range(0.5f, 35f)]
        [Tooltip("The speed at which the crosshairs will change their alpha.")]
        private float m_AlphaLerpSpeed = 5f;

        private readonly List<CrosshairBaseUI> m_Crosshairs = new();
        private CrosshairBaseUI m_CurrentCrosshair;

        private ICrosshairHandler m_CrosshairHandler;
        private IInteractionHandler m_InteractionHandler;
        private IWieldablesController m_WieldableController;
        private bool m_CrosshairEnabled;
        private float m_CrosshairSize;


        protected override void OnAttachment()
        {
            GetModule(out m_InteractionHandler);
            GetModule(out m_WieldableController);

            SetupCrosshairs();

            m_WieldableController.WieldableEquipStarted += OnWieldableChanged;
            OnWieldableChanged(m_WieldableController.ActiveWieldable);

            UpdateManager.AddFixedUpdate(UpdateCrosshair);
        }

        protected override void OnDetachment()
        {
            m_WieldableController.WieldableEquipStarted -= OnWieldableChanged;

            UpdateManager.RemoveFixedUpdate(UpdateCrosshair);
        }

        private void SetupCrosshairs()
        {
            m_Crosshairs.Add(m_DefaultCrosshair);

            foreach (var crosshair in GetComponentsInChildren<BasicCrosshairUI>(true))
            {
                if (!m_Crosshairs.Contains(crosshair))
                    m_Crosshairs.Add(crosshair);

                crosshair.Hide();
            }
        }

        private void OnWieldableChanged(IWieldable wieldable)
        {
            if (m_CrosshairHandler != null)
                m_CrosshairHandler.CrosshairChanged -= SetCrosshair;

            if (wieldable != null)
            {
                m_CrosshairHandler = wieldable as ICrosshairHandler;

                if (m_CrosshairHandler != null)
                {
                    m_CrosshairHandler.CrosshairChanged += SetCrosshair;
                    SetCrosshair(m_CrosshairHandler.CrosshairIndex);
                    return;
                }
            }
            
            m_CrosshairHandler = null;
            SetCrosshair(0);
        }

        private void SetCrosshair(int crosshairIndex)
        {
            var crosshairToEnable = crosshairIndex < 0 ? null : m_Crosshairs[Mathf.Clamp(crosshairIndex, 0, m_Crosshairs.Count - 1)];
            m_CrosshairEnabled = crosshairToEnable != null;

            if (crosshairToEnable != m_CurrentCrosshair && m_CrosshairEnabled)
            {
                if (m_CurrentCrosshair != null)
                    m_CurrentCrosshair.Hide();

                m_CurrentCrosshair = crosshairToEnable;

                if (m_CurrentCrosshair != null)
                {
                    m_CanvasGroup.alpha = 0f;
                    m_CrosshairSize = 0f;
                    m_CurrentCrosshair.Show();
                    m_CurrentCrosshair.SetColor(GameplaySettings.Instance.CrosshairColor);
                }
            }
        }

        private void UpdateCrosshair(float deltaTime)
        {
            if (m_CurrentCrosshair == null)
                return;

            bool enabled = m_CrosshairEnabled && CrosshairEnabled && !m_InteractionHandler.HoverInfo.IsHoverable && !InputManager.HasEscapeCallbacks;
            m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, enabled ? 1f : 0f * m_CrosshairAlpha, deltaTime * m_AlphaLerpSpeed);

            m_CrosshairSize = Mathf.Lerp(m_CrosshairSize, enabled ? 1f : 0.5f, deltaTime * m_AlphaLerpSpeed * 0.5f);
            m_CurrentCrosshair.SetSizeMod(m_CrosshairSize);

            if (m_CrosshairHandler != null)
                m_CurrentCrosshair.SetSize(m_CrosshairHandler.Accuracy);
        }
    }
}