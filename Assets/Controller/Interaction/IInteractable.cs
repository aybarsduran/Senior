using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public interface IInteractable
    {
        GameObject gameObject { get; }
        Transform transform { get; }

        bool InteractionEnabled { get; set; }
        float HoldDuration { get; }

        event UnityAction<ICharacter> Interacted;
        event UnityAction InteractionEnabledChanged;


        /// <summary>
        /// Called when a character interacts with this object.
        /// </summary>
        void OnInteract(ICharacter character);
    }
}