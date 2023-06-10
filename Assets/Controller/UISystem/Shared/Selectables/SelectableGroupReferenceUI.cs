using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.UISystem
{
    public class SelectableGroupReferenceUI : SelectableGroupBaseUI
    {
        public override SelectableUI Selected => m_ReferencedGroup.Selected;
        public override SelectableUI Highlighted => m_ReferencedGroup.Highlighted;
        public override List<SelectableUI> RegisteredSelectables => m_ReferencedGroup.RegisteredSelectables;

        public override event UnityAction<SelectableUI> SelectedChanged
        {
            add => m_ReferencedGroup.SelectedChanged += value;
            remove => m_ReferencedGroup.SelectedChanged -= value;
        }

        public override event UnityAction<SelectableUI> HighlightedChanged
        {
            add => m_ReferencedGroup.HighlightedChanged += value;
            remove => m_ReferencedGroup.HighlightedChanged -= value;
        }

        [SerializeField]
        private SelectableGroupUI m_ReferencedGroup;


        public override void RegisterSelectable(SelectableUI selectable) => m_ReferencedGroup.RegisterSelectable(selectable);
        public override void UnregisterSelectable(SelectableUI selectable) => m_ReferencedGroup.UnregisterSelectable(selectable);
        public override void SelectSelectable(SelectableUI selectable) => m_ReferencedGroup.SelectSelectable(selectable);
        public override void HighlightSelectable(SelectableUI selectable) => m_ReferencedGroup.HighlightSelectable(selectable);
    }
}
