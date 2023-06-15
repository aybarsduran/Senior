using UnityEngine.Events;

namespace IdenticalStudios
{
    public interface IInteractionHandler : ICharacterModule
    {
        bool InteractionEnabled { get; set; }
        HoverInfo HoverInfo { get; }
        float HoveredObjectDistance { get; }

        /// <summary>
        /// Interaction progress 0 - 1 Range
        /// </summary>
        float InteractProgress { get; }

        event UnityAction<HoverInfo> HoverInfoChanged;
        event UnityAction<float> InteractProgressChanged;
        event UnityAction<IInteractable> Interacted;
        event UnityAction<bool> InteractionEnabledChanged;

        void StartInteraction();
        void StopInteraction();
    }
}