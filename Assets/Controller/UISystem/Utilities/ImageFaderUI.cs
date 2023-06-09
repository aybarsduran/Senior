using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    [System.Serializable]
    public sealed class ImageFaderUI
    {
        public bool Fading { get; private set; }
        public Image Image { get => m_Image; set => m_Image = value; }

        [SerializeField]
        private Image m_Image = null;

        [SerializeField, Range(0f, 1f)]
        private float m_MinAlpha = 0.4f;

        [SerializeField, Range(0f, 100f)]
        private float m_FadeInSpeed = 25f;

        [SerializeField, Range(0f, 100f)]
        private float m_FadeOutSpeed = 0.3f;

        [SerializeField, Range(0f, 10f)]
        private float m_FadeOutPause = 0.5f;

        private Coroutine m_FadeHandler;


        public void DoFadeCycle(MonoBehaviour parent, float targetAlpha)
        {
            if (m_Image == null)
            {
                Debug.LogError("[ImageFader] - The image to fade is not assigned!");
                return;
            }

            targetAlpha = Mathf.Clamp01(Mathf.Max(Mathf.Abs(targetAlpha), m_MinAlpha));

            if (m_FadeHandler != null)
                parent.StopCoroutine(m_FadeHandler);

            m_FadeHandler = parent.StartCoroutine(C_DoFadeCycle(targetAlpha));
        }

        private IEnumerator C_DoFadeCycle(float targetAlpha)
        {
            Fading = true;

            while (Mathf.Abs(m_Image.color.a - targetAlpha) > 0.01f)
            {
                m_Image.color = Color.Lerp(m_Image.color, new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, targetAlpha), m_FadeInSpeed * Time.deltaTime);
                yield return null;
            }

            m_Image.color = new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, targetAlpha);

            if (m_FadeOutPause > 0f)
                yield return new WaitForSeconds(m_FadeOutPause);

            while (m_Image.color.a > 0.01f)
            {
                m_Image.color = Color.Lerp(m_Image.color, new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, 0f), m_FadeOutSpeed * Time.deltaTime);
                yield return null;
            }

            m_Image.color = new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, 0f);

            Fading = false;
        }
    }
}
