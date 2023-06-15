using UnityEngine;
using IdenticalStudios.ProceduralMotion;

namespace IdenticalStudios.Demo
{
    public class MessagePopup : MonoBehaviour
    {
        [Title("Display")]

        [SerializeField]
        private SpriteRenderer m_Icon;

        [SerializeField]
        private Color m_MessageSeenIconColor;

        [Title("Animation")]

        [SerializeField]
        private TweenSequence m_ShowAnimation;

        [SerializeField]
        private TweenSequence m_HideAnimation;

        [Title("Audio")]

        [SerializeField]
        private SoundPlayer m_ProximityEnterAudio;

        [SerializeField]
        private SoundPlayer m_ProxmityExitAudio; 

        private bool m_PreviouslyInProximity;


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                m_ProximityEnterAudio.Play2D();

                m_PreviouslyInProximity = true;
                m_ShowAnimation.PlayAnimation();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (m_PreviouslyInProximity)
                    m_Icon.color = new Color(m_MessageSeenIconColor.r, m_MessageSeenIconColor.g, m_MessageSeenIconColor.b, m_Icon.color.a);

                m_ProxmityExitAudio.Play2D();
                m_HideAnimation.PlayAnimation();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_ShowAnimation?.OnValidate(gameObject);
            m_HideAnimation?.OnValidate(gameObject);
        }
#endif
    }
}
