using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.UISystem
{
    [DisallowMultipleComponent]
    public class SelectableGroupUI : SelectableGroupBaseUI
    {
        #region Internal
        protected enum SelectableRegisterMode
        {
            None = 0,
            Disable = 2,
        }
        #endregion

        public override List<SelectableUI> RegisteredSelectables => m_Selectables;

        public override SelectableUI Selected => m_Selected;
        public override SelectableUI Highlighted => m_Highlighted;

        public override event UnityAction<SelectableUI> SelectedChanged;
        public override event UnityAction<SelectableUI> HighlightedChanged;

        [SerializeField]
        private SelectableRegisterMode m_SelectableRegisterMode = SelectableRegisterMode.None;

        protected readonly List<SelectableUI> m_Selectables = new();
        protected SelectableUI m_Selected;
        protected SelectableUI m_Highlighted;


        public override void RegisterSelectable(SelectableUI selectable)
        {
            if (selectable == null)
                return;

            m_Selectables.Add(selectable);

            if (m_SelectableRegisterMode == SelectableRegisterMode.Disable)
            {
                DisableSelectable(selectable);
                return;
            }
        }

        public override void UnregisterSelectable(SelectableUI selectable)
        {
            if (selectable == null)
                return;

            m_Selectables.Remove(selectable);
        }

        public override void SelectSelectable(SelectableUI selectable)
        {
            if (selectable == m_Selected)
                return;

            var prevSelectable = m_Selected;
            m_Selected = selectable;

            if (prevSelectable != null)
                prevSelectable.Deselect();

            if (selectable != null)
                selectable.Select();

            OnSelectedChanged(selectable);
            SelectedChanged?.Invoke(selectable);
        }

        public override void HighlightSelectable(SelectableUI selectable)
        {
            m_Highlighted = selectable;
            HighlightedChanged?.Invoke(selectable);
        }

        protected virtual void OnSelectedChanged(SelectableUI selectable) { }
    }
}
