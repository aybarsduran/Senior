using UnityEngine.EventSystems;

namespace IdenticalStudios.UISystem
{
    public class ButtonUI : SelectableUI
    {
        //public override void OnPointerDown(PointerEventData eventData)
        //{
        //    if (!m_IsSelectable || !m_IsInteractable)
        //        return;

        //    if (eventData.button != PointerEventData.InputButton.Left)
        //        return;

        //    base.OnPointerDown(eventData);
        //    m_OnSelected.Invoke(this);
        //}

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            base.OnPointerUp(eventData);
            m_OnSelected.Invoke(this);
        }

        public override void OnSelect(BaseEventData eventData) { }
    }
}
