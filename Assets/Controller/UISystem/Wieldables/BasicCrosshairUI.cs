using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    [RequireComponent(typeof(RectTransform))]
    public class BasicCrosshairUI : CrosshairBaseUI
    {
        [SerializeField, HideInInspector]
        private RectTransform m_RectTransform;

        [SerializeField, Range(0f, 100f)]
        private float m_MinSize = 10f;

        [SerializeField, Range(0f, 1000f)]
        private float m_MaxAccuracySize;

        [SerializeField, Range(0f, 1000f)]
        private float m_MinAccuracySize;

        [SerializeField, HideInInspector]
        private Image[] m_CrosshairImages;

        private float m_SizeMod = 1f;
        
        
        public override void SetSize(float accuracy)
        {
            float size = Mathf.Lerp(m_MinAccuracySize, m_MaxAccuracySize, Mathf.Clamp01(accuracy));
            size = Mathf.Max(m_MinSize, size * m_SizeMod);
            m_RectTransform.sizeDelta = new Vector2(size, size);
        }

        public override void SetSizeMod(float mod) => m_SizeMod = mod;

        public override void SetColor(Color color)
        {
            foreach (Image image in m_CrosshairImages)
                image.color = color;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_CrosshairImages = GetComponentsInChildren<Image>();
            m_RectTransform = GetComponent<RectTransform>();

            UnityUtils.SafeOnValidate(this, () => { SetSize(m_MinAccuracySize); });
        }
#endif
    }
}