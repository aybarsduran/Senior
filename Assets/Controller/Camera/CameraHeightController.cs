using IdenticalStudios.ProceduralMotion;
using UnityEngine;

namespace IdenticalStudios
{
    public sealed class CameraHeightController : CharacterBehaviour
    {
        [System.Serializable]
        public struct EasingOptions
        {
            public EaseType EaseType;
            public float Duration;
        }

        [SerializeField, Range(0f, 100f)]
        private float m_EyeHeight = 1.7f;


        [SerializeField, Range(5f, 50f)]
        [Tooltip("How fast should the camera adjust to the current Y position. (up - down)")]
        private float m_YLerpSpeed = 20f;

        [SerializeField]
        private EaseType m_HeightChangeEase = EaseType.QuartInOut;

        [SerializeField, Range(0.001f, 10f)]
        private float m_HeightChangeDuration = 0.33f;

        private ICharacterMotor m_Motor;
        private float m_EyeHeightOffset;
        private float m_LastYPosition;
        private float m_LerpSpeed;
        private Tween<float> m_Tween;


        protected override void OnBehaviourEnabled()
        {
            transform.localPosition = new Vector3(0f, m_EyeHeight, 0f);

            m_Tween = gameObject.TweenValueFloat(m_EyeHeight, m_HeightChangeDuration, (float value) => m_EyeHeightOffset = value - m_EyeHeight)
                .SetFrom(m_EyeHeight)
                .SetEase(m_HeightChangeEase);

            GetModule(out m_Motor);
            m_Motor.HeightChanged += SetHeight;
            //SetHeight(m_Motor.Height);

            GetModule<ILookHandler>().PostViewUpdate += UpdatePosition;

            m_LastYPosition = m_Motor.transform.position.y - m_Motor.DefaultHeight + (m_EyeHeight * 2f);
        }

        protected override void OnBehaviourDisabled()
        {
            m_Motor.HeightChanged -= SetHeight;
            GetModule<ILookHandler>().PostViewUpdate -= UpdatePosition;
        }

        private void UpdatePosition()
        {
            float deltaTime = Time.unscaledDeltaTime;

            m_LerpSpeed = Mathf.Lerp(m_LerpSpeed, m_Motor.IsGrounded ? m_YLerpSpeed : 100f, deltaTime * 10f);

            m_LastYPosition = Mathf.Lerp(m_LastYPosition, m_Motor.transform.position.y - m_Motor.DefaultHeight + (m_EyeHeight * 2f), m_LerpSpeed * deltaTime);
            transform.position = new Vector3(transform.position.x, m_LastYPosition + m_EyeHeightOffset, transform.position.z);
        }

        private void SetHeight(float height)
        {
            m_Tween.SetTargetValue(height);
            m_Tween.Play();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_Tween != null)
            {
                m_Tween.SetDuration(m_HeightChangeDuration);
                m_Tween.SetEase(m_HeightChangeEase);
            }
        }
#endif
    }
}