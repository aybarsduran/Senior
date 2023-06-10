using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace IdenticalStudios.UISystem
{
    public class SelectableTabUI : SelectableUI
    {
        #region Internal
        [System.Flags]
        protected enum SelectMode
        {
            None = 0,
            EnablePanel = 1,
            EnableObject = 2,
            Everything = EnablePanel | EnableObject
        }
        #endregion

        public string TabName
        {
            get
            {
                if (m_NameText == null)
                    return string.Empty;

                return m_NameText.text;
            }
            set
            {
                if (m_NameText != null)
                    m_NameText.text = value;
            }
        }

        public event UnityAction<SelectableUI> OnDeselected
        {
            add => m_OnDeselected.AddListener(value);
            remove => m_OnDeselected.RemoveListener(value);
        }

        [SerializeField]
        protected SelectedEvent m_OnDeselected;

        [SerializeField]
        protected TextMeshProUGUI m_NameText;


        [SerializeField]
        protected SelectMode m_SelectMode;

        [SerializeField]
        protected PanelUI m_PanelToEnable;

        [SerializeField]
        protected GameObject m_ObjectToEnable;


        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            if (m_IsSelected)
            {
                if ((m_SelectMode & SelectMode.EnablePanel) == SelectMode.EnablePanel)
                    m_PanelToEnable.Show(true);

                if ((m_SelectMode & SelectMode.EnableObject) == SelectMode.EnableObject)
                    m_ObjectToEnable.SetActive(true);
            }
        }

        public override void Deselect()
        {
            if (!m_IsSelected)
                return;

            base.Deselect();

            if ((m_SelectMode & SelectMode.EnablePanel) == SelectMode.EnablePanel)
                m_PanelToEnable.Show(false);

            if ((m_SelectMode & SelectMode.EnableObject) == SelectMode.EnableObject)
                m_ObjectToEnable.SetActive(false);

            m_OnDeselected.Invoke(this);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_NameText == null)
                m_NameText = GetComponentInChildren<TextMeshProUGUI>();
        }
#endif
    }
}
