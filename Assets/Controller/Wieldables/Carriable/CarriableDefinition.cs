using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [CreateAssetMenu(menuName = "Identical Studios/Building/Carriable Definition", fileName = "(Carriable) ")]
    public sealed class CarriableDefinition : DataDefinition<CarriableDefinition>
    {
        public override string Name
        {
            get => m_CarriableName;
            protected set => m_CarriableName = value;
        }

        public Carriable Wieldable => m_Wieldable;
        public CarriablePickup Pickup => m_Pickup;

        [SerializeField]
        [NewLabel("Name ")]
        [Tooltip("Carriable name.")]
        private string m_CarriableName;

        [SerializeField, Line]
        [AssetPreview, NotNull, PrefabObjectOnly]
        [Tooltip("Corresponding Wieldable")]
        private Carriable m_Wieldable;

        [SerializeField, Line]
        [AssetPreview, NotNull, PrefabObjectOnly]
        [Tooltip("Corresponding pickup for this carriable, so you can actually drop it, or pick it up from the ground.")]
        private CarriablePickup m_Pickup;
    }
}