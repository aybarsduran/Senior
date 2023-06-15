using IdenticalStudios.InventorySystem;
using UnityEngine;

namespace IdenticalStudios.Demo
{
    public class AddItemToInventory : MonoBehaviour
    {
        [SerializeField, DataReferenceDetails(HasNullElement = false), ReorderableList(HasLabels = false)]
        private DataIdReference<ItemCategoryDefinition>[] m_Categories;


        public void AddItemToCharacter(ICharacter character) 
        {
            var category = m_Categories.SelectRandom().Def;
            Item itemToAdd = new Item(category.Members.SelectRandom());
            character.Inventory.AddItem(itemToAdd);
        }

        public void AddItemToCollider(Collider collider)
        {
            if (collider.TryGetComponent(out ICharacter character))
                AddItemToCharacter(character);
        }
    }
}