using IdenticalStudios.WorldManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace IdenticalStudios
{
    /// <summary>
    /// Handles the Visual Effects of a camera.
    /// </summary>
    public class CameraEffectsHandler : CharacterBehaviour, ICameraEffectsHandler
    {
#if UNITY_POST_PROCESSING_STACK_V2
        private ColorGrading m_ColorGrading;
        private Vignette m_Vignette;
        private ChromaticAberration m_ChromaticAberration;

        private float m_DefaultExposure;
        private float m_DefaultSaturation;
        private float m_DefaultVignette;
        private float m_DefaultChromaticAberration;
#endif

        private Coroutine m_AnimationCoroutine;


        protected override void OnBehaviourEnabled()
        {
#if UNITY_POST_PROCESSING_STACK_V2
            if (PostProcessingManager.Instance == null)
                return;

            PostProcessingManager.OverlayVolume.profile.TryGetSettings(out m_ColorGrading);
            PostProcessingManager.OverlayVolume.profile.TryGetSettings(out m_Vignette);
            PostProcessingManager.OverlayVolume.profile.TryGetSettings(out m_ChromaticAberration);

            m_DefaultExposure = m_ColorGrading.postExposure.value;
            m_DefaultSaturation = m_ColorGrading.saturation.value;
            m_DefaultVignette = m_Vignette.intensity.value;
            m_DefaultChromaticAberration = m_ChromaticAberration.intensity.value;
#endif
        }

        public void DoAnimationEffect(CameraEffectSettings effect)
        {
            if (effect == null)
                return;

            if (m_AnimationCoroutine != null)
            {
                StopCoroutine(m_AnimationCoroutine);
                ResetPostProcessingValues();
            }

            m_AnimationCoroutine = StartCoroutine(C_DoAnimatedEffect(effect));
        }

        private void ResetPostProcessingValues()
        {
#if UNITY_POST_PROCESSING_STACK_V2
            m_ColorGrading.postExposure.value = m_DefaultExposure;
            m_ColorGrading.saturation.value = m_DefaultSaturation;
            m_Vignette.intensity.value = m_DefaultVignette;
            m_ChromaticAberration.intensity.value = m_DefaultChromaticAberration;
#endif
        }

        private IEnumerator C_DoAnimatedEffect(CameraEffectSettings effects)
        {
            // Check if the module is initialized
            if (!IsBehaviourEnabled)
                yield break;

            float EndTime = Time.time + effects.Duration;
            float animTimer = 0f;

            while (animTimer < 1f)
            {
                animTimer = 1 - ((EndTime - Time.time) / effects.Duration);

#if UNITY_POST_PROCESSING_STACK_V2
                if (effects.ExposureEffect.Enabled)
                    UpdateEffect(m_ColorGrading.postExposure, effects.ExposureEffect, animTimer, m_DefaultExposure);

                if (effects.SaturationEffect.Enabled)
                    UpdateEffect(m_ColorGrading.saturation, effects.SaturationEffect, animTimer, m_DefaultSaturation);

                if (effects.VignetteEffect.Enabled)
                    UpdateEffect(m_Vignette.intensity, effects.VignetteEffect, animTimer, m_DefaultVignette);

                if (effects.ChromaticAberationEffect.Enabled)
                    UpdateEffect(m_ChromaticAberration.intensity, effects.ChromaticAberationEffect, animTimer, m_DefaultChromaticAberration);
#endif

                yield return null;
            }

            ResetPostProcessingValues();
        }

#if UNITY_POST_PROCESSING_STACK_V2
        private static void UpdateEffect(FloatParameter value, CameraEffectSettings.Effect effect, float animTime, float originalValue)
        {
            value.value = originalValue + animTime * effect.ValueChange * effect.ValueChangeOverTime.Evaluate(animTime);
        }
#endif
    }
}