using UnityEngine.Events;

namespace IdenticalStudios.UISystem
{
    public abstract class ItemSelectorUI : PlayerUIBehaviour
    {
        public static ItemSlotUI SelectedSlot
        {
            get => s_SelectedSlot;
            protected set
            {
                if (s_SelectedSlot == value)
                    return;

                s_SelectedSlot = value;
                SelectedSlotChanged?.Invoke(value);
            }
        }

        public static ItemSlotUI HighlightedSlot
        {
            get => s_HighlightedSlot;
            protected set
            {
                if (s_HighlightedSlot == value)
                    return;

                s_HighlightedSlot = value;
                HighlightedSlotChanged?.Invoke(value);
            }
        }

        public static event UnityAction<ItemSlotUI> SelectedSlotChanged;
        public static event UnityAction<ItemSlotUI> HighlightedSlotChanged;

        protected static ItemSlotUI s_SelectedSlot;
        protected static ItemSlotUI s_HighlightedSlot;


        protected override void OnAttachment()
        {
            var inspection = GetModule<IInventoryInspectManager>();
            inspection.AfterInspectionStarted += OnInspectionStarted;
            inspection.BeforeInspectionEnded += OnInspectionEnded;
        }

        protected override void OnDetachment()
        {
            var inspection = GetModule<IInventoryInspectManager>();
            inspection.AfterInspectionStarted -= OnInspectionStarted;
            inspection.BeforeInspectionEnded -= OnInspectionEnded;
        }

        protected abstract void OnInspectionStarted();
        protected abstract void OnInspectionEnded();

        protected void RaiseSelectedEvent() => SelectedSlotChanged?.Invoke(SelectedSlot);
        protected void RaiseHighlightedEvent() => HighlightedSlotChanged?.Invoke(HighlightedSlot);
    }
}
