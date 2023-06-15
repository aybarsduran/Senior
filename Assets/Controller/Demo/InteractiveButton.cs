using IdenticalStudios.ProceduralMotion;
using UnityEngine;

namespace IdenticalStudios.Demo
{
    public class InteractiveButton : Interactable
    {
        [Title("Settings (Interactive Button)")]

        [SerializeField, Range(0f, 10f)]
        private float m_PressThreshold = 0.3f;

        [SerializeField]
        private bool m_HasPressLimit;

        [SerializeField, ShowIf(nameof(m_HasPressLimit), true), Range(0, 100)]
        private int m_MaxPressTimes = 0;

        [SerializeField]
        private TweenSequence m_PressAnimation;

        [SpaceArea]

        [SerializeField]
        private SoundPlayer m_PressAudio;

        private int m_PressedTimes;
        private float m_NextTimeCanPress;


        public override void OnInteract(ICharacter character)
        {
            if (Time.time < m_NextTimeCanPress || (m_HasPressLimit && m_PressedTimes >= m_MaxPressTimes))
                return;

            m_NextTimeCanPress = Time.time + m_PressThreshold;
            m_PressedTimes++;

            m_PressAnimation.PlayAnimation();
            m_PressAudio.PlayAtPosition(transform.position);

            base.OnInteract(character);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            m_PressAnimation.OnValidate(gameObject);
        }
#endif
    }
}