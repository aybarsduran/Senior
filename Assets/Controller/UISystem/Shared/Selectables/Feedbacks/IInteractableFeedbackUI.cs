namespace IdenticalStudios.UISystem
{
    public interface IInteractableFeedbackUI
    {
        void OnNormal(bool instant);
        void OnHighlighted(bool instant);
        void OnSelected(bool instant);
        void OnPressed(bool instant);

#if UNITY_EDITOR
        void OnValidate(SelectableUI selectable);
#endif
    }
}