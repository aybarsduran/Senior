using IdenticalStudios.WieldableSystem;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public class ChargeUI : PlayerUIBehaviour
    {
        [SerializeField]
        [Tooltip("The canvas group used to fade the stamina bar in & out.")]
        private CanvasGroup m_CanvasGroup;

        [SerializeField]
        [Tooltip("UI images that will have their fill amount value set to the current charge value.")]
        private Image[] m_ChargeFillImages;

        [SerializeField]
        [Tooltip("A gradient used in determining the color of the charge image relative to the current charge value.")]
        private Gradient m_FillGradient;

        private IChargeHandler m_ChargeHandler;
        private IFirearm m_Firearm;


        protected override void OnAttachment()
        {
            GetModule<IWieldablesController>().WieldableEquipStopped += OnWieldableEquipped;
            m_CanvasGroup.alpha = 0f;
        }

        protected override void OnDetachment()
        {
            GetModule<IWieldablesController>().WieldableEquipStopped -= OnWieldableEquipped;
        }

        private void OnWieldableEquipped(IWieldable wieldable)
        {
            // Unsubscribe from previous firearm
            if (m_Firearm != null)
            {
                m_Firearm.TriggerChanged -= OnTriggerChanged;
                m_Firearm = null;
                OnTriggerChanged(null);
            }

            if (wieldable is IFirearm firearm)
            {
                // Subscribe to current firearm
                m_Firearm = firearm;
                m_Firearm.TriggerChanged += OnTriggerChanged;
                OnTriggerChanged(m_Firearm.Trigger);
            }
        }

        private void UpdateCharge(float deltaTime)
        {
            float normalizedCharge = m_ChargeHandler.GetNormalizedCharge();
            UpdateChargeImages(normalizedCharge);

            bool showCanvas = normalizedCharge > 0.01f;

            m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, showCanvas ? 1f : 0f, deltaTime * 3f);
        }

        private void UpdateChargeImages(float fillAmount) 
        {
            Color chargeColor = m_FillGradient.Evaluate(fillAmount);

            for (int i = 0; i < m_ChargeFillImages.Length; i++)
            {
                m_ChargeFillImages[i].fillAmount = fillAmount;
                m_ChargeFillImages[i].color = chargeColor;
            }
        }

        private void OnTriggerChanged(ITrigger currentTrigger)
        {
            var prevCharge = m_ChargeHandler;
            m_ChargeHandler = currentTrigger as IChargeHandler;

            if (m_ChargeHandler != null && prevCharge == null)
            {
                UpdateManager.AddUpdate(UpdateCharge);
                return;
            }

            if (m_ChargeHandler == null && prevCharge != null)
            {
                UpdateManager.RemoveUpdate(UpdateCharge);
                UpdateChargeImages(0f);
                m_CanvasGroup.alpha = 0f;
                return;
            }
        }
    }
}