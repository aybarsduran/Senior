using IdenticalStudios.ProceduralMotion;
using UnityEngine;

namespace PolymindGames.ProceduralMotion
{
    [ExecuteAlways]
    public sealed class TweenAnimation : MonoBehaviour
    {
        [SerializeField]
        private bool m_PlayOnStart = false;

        [SerializeField]
        private TweenSequence m_TweenSequence = new();


        public void PlayAnimation() => m_TweenSequence.PlayAnimation();
        public void CancelAnimation() => m_TweenSequence.CancelAnimation();

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            {
                if (m_PlayOnStart)
                    m_TweenSequence.PlayAnimation();
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
                m_TweenSequence.OnValidate(gameObject);
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
                m_TweenSequence.CancelAnimation();

#if UNITY_EDITOR
            if (!Application.isPlaying)
                m_TweenSequence.OnValidate(gameObject);
#endif
        }

#if UNITY_EDITOR
        private void OnValidate() => m_TweenSequence.OnValidate(gameObject);
#endif
    }
}
