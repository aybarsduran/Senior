using IdenticalStudios.WorldManagement;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/FP Field Of View")]
    public class FPWieldableFOV : WieldableBehaviour
    {
        [SerializeField, Range(10f, 100f), NewLabel("Overlay FOV")]
        private float m_BaseOverlayFOV = 55f;

        [SerializeField, Range(0f, 10f)]
        private float m_DOFMultiplier = 1f;

        private ICameraFOVHandler m_FOVHandler;
        
        
        protected override void OnEquippingStarted()
        {
            Wieldable.Character.GetModule(out m_FOVHandler);
            m_FOVHandler.SetCustomOverlayFOV(m_BaseOverlayFOV, 0f);

            if (PostProcessingManager.HasInstance)
                PostProcessingManager.Instance.EnableDOF(false);
        }

        protected override void OnHolsteringEnded()
        {
            if (PostProcessingManager.HasInstance)
                PostProcessingManager.Instance.EnableDOF(false);
        }

        public void SetDOFMod(float mod)
        {
            if (PostProcessingManager.HasInstance)
            {
                PostProcessingManager.Instance.EnableDOF(m_DOFMultiplier * mod > 0.01f);
                PostProcessingManager.Instance.SetDOFFocalLength(12f * mod * m_DOFMultiplier);
            }
        }

        public void SetOverlayFOV(float fovMod) => SetOverlayFOV(fovMod, 0.5f);
        public void SetWorldFOV(float fovMod) => SetWorldFOV(fovMod, 0.5f);

        public void SetOverlayFOV(float fovMod, float duration, float delay = 0f)
        {
            m_FOVHandler.SetCustomOverlayFOV(m_BaseOverlayFOV * fovMod, duration, delay);
        }

        public void SetWorldFOV(float fovMod, float duration, float delay = 0f)
        {
            m_FOVHandler.SetCustomWorldFOVMod(fovMod, duration, delay);
        }
    }
}
