using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public class CarriablePickup : Interactable
    {
        [SpaceArea]

        [SerializeField, DataReferenceDetails(HasNullElement = false, HasAssetReference = true)]
        [Tooltip("The corresponding carriable definition.")]
        private DataIdReference<CarriableDefinition> m_Definition;


        public override void OnInteract(ICharacter character)
        {
            if (character.TryGetModule(out IWieldableCarriableHandler objectCarry))
            {
                if (objectCarry.TryCarryObject(m_Definition))
                {
                    base.OnInteract(character);
                    Destroy(gameObject);
                }
            }
        }

        private void Start()
        {
            Title = "Carry";
            Description = m_Definition.Name;
        }
    }
}