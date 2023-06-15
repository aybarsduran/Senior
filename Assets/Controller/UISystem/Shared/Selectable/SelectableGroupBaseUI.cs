using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.UISystem
{
    public abstract class SelectableGroupBaseUI : MonoBehaviour
    {
        public abstract SelectableUI Selected { get; }
        public abstract SelectableUI Highlighted { get; }
        public abstract List<SelectableUI> RegisteredSelectables { get; }

        public abstract event UnityAction<SelectableUI> SelectedChanged;
        public abstract event UnityAction<SelectableUI> HighlightedChanged;


        public abstract void RegisterSelectable(SelectableUI selectable);
        public abstract void UnregisterSelectable(SelectableUI selectable);
        public abstract void SelectSelectable(SelectableUI selectable);
        public abstract void HighlightSelectable(SelectableUI selectable);

        public void EnableAllSelectables()
        {
            var selectables = RegisteredSelectables;
            for (int i = 0; i < selectables.Count; i++)
                EnableSelectable(selectables[i]);

            if (Selected == null && selectables.Count > 0)
                selectables[0].Select();
        }

        public void DisableAllSelectables()
        {
            var selectables = RegisteredSelectables;
            for (int i = 0; i < selectables.Count; i++)
                DisableSelectable(selectables[i]);
        }

        public void EnableSelectedAndSelectIfNoSelected(SelectableUI selectable)
        {
            EnableSelectable(selectable);
            if (Selected == null || !Selected.isActiveAndEnabled)
                selectable.OnSelect(null);
        }

        public int GetIndexOfSelectable(SelectableUI selectable)
        {
            for (int i = 0; i < RegisteredSelectables.Count; i++)
            {
                if (RegisteredSelectables[i] == selectable)
                    return i;
            }

            return -1;
        }

        public void EnableSelectable(SelectableUI selectable) => selectable.gameObject.SetActive(true);
        public void DisableSelectable(SelectableUI selectable) => selectable.gameObject.SetActive(false);
    }
}
