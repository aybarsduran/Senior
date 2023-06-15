using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public interface IHoverable
    {
        GameObject gameObject { get; }
        Transform transform { get; }

        bool IsHoverable { get; }

        string Title { get; }
        string Description { get; }

        event UnityAction<ICharacter> HoverStarted;
        event UnityAction<ICharacter> HoverEnded;
        event UnityAction DescriptionChanged;


        /// <summary>
        /// Called when a character starts looking at this object.
        /// </summary>
        void OnHoverStart(ICharacter character);

        /// <summary>
        /// Called when a character stops looking at this object.
        /// </summary>
        void OnHoverEnd(ICharacter character);
    }

    public struct HoverInfo
    {
        public bool IsHoverable => Hoverable != null && Hoverable.IsHoverable;
        public static HoverInfo Default => s_Default;

        public readonly Collider Collider;
        public readonly IHoverable Hoverable;

        private static readonly HoverInfo s_Default = new(null, null);


        public HoverInfo(Collider collider, IHoverable hoverable)
        {
            Collider = collider;
            Hoverable = hoverable;
        }
    }
}
