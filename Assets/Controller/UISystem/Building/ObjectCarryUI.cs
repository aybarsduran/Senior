using System.Collections;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public sealed class ObjectCarryUI : PlayerUIBehaviour
    {
        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        [SerializeField, Range(0.1f, 20f)]
        private float m_AlphaLerpSpeed = 7f;

        private IWieldableCarriableHandler m_ObjectCarry;
        private float m_TargetAlpha;

        private Coroutine m_FadeCoroutine;


        protected override void OnAttachment()
        {
            GetModule(out m_ObjectCarry);

            m_ObjectCarry.ObjectCarryStarted += OnObjectCarryStart;
            m_ObjectCarry.ObjectCarryStopped += OnObjectCarryEnd;
        }

        private void OnObjectCarryEnd()
        {
            m_TargetAlpha = 0f;

            if (m_FadeCoroutine == null)
                StartCoroutine(C_FadeInterface());
        }

        private void OnObjectCarryStart()
        {
            m_TargetAlpha = 1f;

            if (m_FadeCoroutine == null)
                StartCoroutine(C_FadeInterface());
        }

        private IEnumerator C_FadeInterface() 
        {
            while (!Mathf.Approximately(Mathf.Abs(m_CanvasGroup.alpha - m_TargetAlpha), 0f))
            {
                m_CanvasGroup.alpha = Mathf.MoveTowards(m_CanvasGroup.alpha, m_TargetAlpha, Time.deltaTime * m_AlphaLerpSpeed);
                yield return null;
            }

            m_FadeCoroutine = null;
        }
    }
}