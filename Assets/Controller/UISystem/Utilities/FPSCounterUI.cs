using TMPro;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    [RequireComponent(typeof (TextMeshProUGUI))]
    public sealed class FPSCounterUI : MonoBehaviour
    {
        [SerializeField, Range(30f, 1000f)]
        private float m_RequiredFPS;

        [SerializeField]
        private Gradient m_ColorGradient;

        private const float k_FpsMeasurePeriod = 0.5f;
        private const string k_Display = "{0} FPS";
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        private int m_CurrentFps;
        private TextMeshProUGUI m_Text;


        private void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + k_FpsMeasurePeriod;
            m_Text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            // Measure average frames per second
            m_FpsAccumulator++;

            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int) (m_FpsAccumulator/k_FpsMeasurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += k_FpsMeasurePeriod;

                m_Text.text = string.Format(k_Display, m_CurrentFps);
                m_Text.color = m_ColorGradient.Evaluate(m_CurrentFps / m_RequiredFPS);
            }
        }
    }
}
