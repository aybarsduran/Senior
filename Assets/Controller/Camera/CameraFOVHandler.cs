using IdenticalStudios.ProceduralMotion;
using IdenticalStudios;
using System;
using UnityEngine;

namespace IdenticalStudios
{
    /// <summary>
    /// Handles the World & Overlay FOV of a camera.
    /// </summary>
    public class CameraFOVHandler : CharacterBehaviour, ICameraFOVHandler
    {
        #region Internal
        [Serializable]
        private class CameraFOVState
        {
            public MovementStateType StateType = MovementStateType.Idle;

            [Range(0.1f, 5f)]
            public float Multiplier = 1f;
        }
        #endregion

        public Camera UnityWorldCamera => m_WorldCamera;
        public Camera UnityOverlayCamera => m_OverlayCamera;

        //Settings (World)

        [SerializeField]
        private Camera m_WorldCamera;

        [SerializeField, Range(30f, 120f)]
        private float m_BaseWorldFOV = 90f;

        [SerializeField, Range(0f, 0.5f)]
        private float m_WorldResetDuration = 0.01f;

        [SerializeField]
        private EaseType m_WorldEaseType;

    

        [SerializeField, Range(0.1f, 5f)]
        private float m_AirborneFOVMod = 1.05f;

        [SerializeField]
        private AnimationCurve m_SpeedFOVMultiplier = new(new[] { new Keyframe(0f, 1f), new Keyframe(1f, 1f) });

        [SerializeField]
        private AnimationCurve m_HeightFOVMultiplier = new(new[] { new Keyframe(0f, 1f), new Keyframe(1f, 1f) });

        //Settings (Overlay)

        [SerializeField]
        private Camera m_OverlayCamera;

        [SerializeField, Range(30f, 120f)]
        private float m_BaseOverlayFOV = 50f;

        [SerializeField, Range(0f, 0.5f)]
        private float m_OverlayResetDuration = 0.02f;

        [SerializeField]
        private EaseType m_OverlayEaseType;

        private Tween<float> m_OverlayTween;
        private Tween<float> m_WorldTween;
        private float m_WorldFOVMod = 1f;
        private ICharacterMotor m_Motor;


        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_Motor);

            m_WorldCamera.fieldOfView = m_BaseWorldFOV;
            m_OverlayCamera.fieldOfView = m_BaseOverlayFOV;

            m_WorldTween = gameObject.TweenValueFloat(m_BaseWorldFOV, 0f, null).SetEase(m_WorldEaseType).SetFrom(m_BaseWorldFOV);
            m_OverlayTween = m_OverlayCamera.TweenCameraFieldOfView(m_BaseOverlayFOV, 0f).SetEase(m_OverlayEaseType).SetFrom(m_BaseOverlayFOV);

            m_WorldTween.Play();
            m_OverlayTween.Play();
        }

        public void SetCustomWorldFOVMod(float fovMultiplier, float duration = 0.3f, float delay = 0f)
        {
            float targetFOV = m_BaseWorldFOV * fovMultiplier;
            m_WorldTween.SetTargetValue(targetFOV, duration).SetDelay(delay).Play();

            if (duration < 0.01f)
                m_WorldTween.SetTime(1f);
        }

        public void ResetCustomWorldFOV(bool instantly)
        {
            float duration = Mathf.Abs(m_BaseWorldFOV - m_WorldTween.Value) * m_WorldResetDuration;
            m_WorldTween.SetTargetValue(m_BaseWorldFOV, duration);
            m_WorldTween.Play();

            if (instantly)
                m_WorldTween.SetTime(1f);
        }

        public void SetCustomOverlayFOV(float fov, float duration = 0.01f, float delay = 0f)
        {
            m_OverlayTween.SetTargetValue(fov, duration).SetDelay(delay).Play();

            if (duration < 0.01f)
                m_OverlayTween.SetTime(1f);
        }

        public void ResetCustomOverlayFOV(bool instantly)
        {
            float duration = Mathf.Abs(m_BaseOverlayFOV - m_OverlayTween.Value) * m_OverlayResetDuration;
            m_OverlayTween.SetTargetValue(m_BaseOverlayFOV, duration);
            m_OverlayTween.Play();

            if (instantly)
                m_OverlayTween.SetTime(1f);
        }

        private void Update()
        {
            if (!IsBehaviourEnabled)
                return;

            m_WorldFOVMod = Mathf.Lerp(m_WorldFOVMod, GetWorldFOVMod(), Time.deltaTime * 3f);
            m_WorldCamera.fieldOfView = m_WorldTween.Value * m_WorldFOVMod;
        }

        private float GetWorldFOVMod()
        {
            float multiplier = 1f;

            var velocity = m_Motor.Velocity;
            var horizontalVel = new Vector2(velocity.x, velocity.z);
            multiplier *= m_SpeedFOVMultiplier.Evaluate(horizontalVel.magnitude);
            multiplier *= m_HeightFOVMultiplier.Evaluate(m_Motor.Height);

            if (!m_Motor.IsGrounded)
                multiplier *= m_AirborneFOVMod;

            return multiplier;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            m_OverlayTween?.SetEase(m_OverlayEaseType).SetFrom(m_BaseOverlayFOV);
            m_WorldTween?.SetEase(m_WorldEaseType).SetFrom(m_BaseWorldFOV);
        }
#endif
    }
}