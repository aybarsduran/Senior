using IdenticalStudios.UISystem;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public class FirearmScopedAimer : FirearmBasicAimer
    {

        [SerializeField, Range(0f, 10f)]
        private float m_ScopeEnableDelay = 0.3f;

        [SerializeField, Range(0, 24)]
        private int m_ScopeIndex;


        [SerializeField]
        private Transform m_ObjectToDisable;

        private bool m_HideGun;
        private float m_HideTime;
        private Vector3 m_DefaultGunModelScale;


        public override void StartAim()
        {
            base.StartAim();

            if (!IsAiming)
                return;

            // Scope
            ScopeUI.EnableScope(m_ScopeIndex, m_ScopeEnableDelay);

            m_HideGun = true;
            m_HideTime = Time.time + m_ScopeEnableDelay;
        }

        public override void EndAim()
        {
            base.EndAim();

            if (IsAiming)
                return;

            ScopeUI.DisableScope();

            m_HideGun = false;
            m_ObjectToDisable.transform.localScale = m_DefaultGunModelScale;
        }

        protected override void HandleFOV(FPWieldableFOV fovHandler, bool aim)
        {
            if (!aim)
            {
                fovHandler.SetOverlayFOV(1f, 0f, 0f);
                fovHandler.SetWorldFOV(1f, 0f, 0f);
                fovHandler.SetDOFMod(0f);
            }
            else
                base.HandleFOV(fovHandler, aim);
        }

        protected override void Awake()
        {
            base.Awake();

            m_DefaultGunModelScale = m_ObjectToDisable != null ? m_ObjectToDisable.transform.localScale : Vector3.one;
        }

        private void Update()
        {
            if (m_HideGun && Time.time > m_HideTime)
            {
                m_ObjectToDisable.transform.localScale = Vector3.zero;
                m_HideGun = false;
            }
        }
    }
}