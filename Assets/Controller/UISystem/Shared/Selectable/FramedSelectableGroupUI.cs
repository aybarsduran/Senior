using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    [AddComponentMenu("IdenticalStudios/UserInterface/Selectables/Framed Selectable Group")]
    public class FramedSelectableGroupUI : SelectableGroupUI
    {
        #region Internal
        [System.Flags]
        private enum FrameSelectionMatchType
        {
            MatchColor = 1,
            MatchXSize = 2,
            MatchYSize = 4,
        }
        #endregion

        [Title("Frame Settings")]

        [SerializeField, PrefabObjectOnly, NotNull]
        private RectTransform m_SelectionFrame;

        [SerializeField]
        private Vector2 m_SelectionOffset = Vector2.zero;

        [SerializeField]
        private FrameSelectionMatchType m_SelectionMatchFlags;

        private RectTransform m_Frame;
        private bool m_FrameActive = true;


        protected override void OnSelectedChanged(SelectableUI selectable)
        {
            if (m_Frame == null)
                CreateFrame();

            if (selectable == null || selectable.gameObject.HasComponent(typeof(RaycastMask)))
            {
                EnableFrame(false);
                return;
            }

            EnableFrame(true);

            m_Frame.SetParent(selectable.transform);
            m_Frame.anchoredPosition = m_SelectionOffset;
            m_Frame.localRotation = Quaternion.identity;
            m_Frame.localScale = Vector3.one;
            var localPos = m_Frame.localPosition;
            m_Frame.localPosition = new Vector3(localPos.x, localPos.y, 0f);

            bool matchXSize = (m_SelectionMatchFlags & FrameSelectionMatchType.MatchXSize) == FrameSelectionMatchType.MatchXSize;
            bool matchYSize = (m_SelectionMatchFlags & FrameSelectionMatchType.MatchYSize) == FrameSelectionMatchType.MatchYSize;

            if (matchXSize || matchYSize)
            {
                var frameSize = m_Frame.sizeDelta;
                var selectableSize = ((RectTransform)selectable.transform).sizeDelta;

                m_Frame.sizeDelta = new Vector2(matchXSize ? selectableSize.x : frameSize.x, matchYSize ? selectableSize.y : frameSize.y);
            }

            bool matchColor = (m_SelectionMatchFlags & FrameSelectionMatchType.MatchColor) == FrameSelectionMatchType.MatchColor;

            if (matchColor && m_Frame.TryGetComponent<Graphic>(out var frameGraphic))
                frameGraphic.color = selectable.GetComponent<Graphic>().color;
        }

        private void CreateFrame()
        {
            m_Frame = Instantiate(m_SelectionFrame);
            m_FrameActive = m_Frame.gameObject.activeSelf;
        }

        private void EnableFrame(bool enable)
        {
            if (m_FrameActive == enable)
                return;

            m_Frame.gameObject.SetActive(enable);
            m_FrameActive = enable;
        }
    }
}
