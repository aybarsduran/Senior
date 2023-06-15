using IdenticalStudios.InventorySystem;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Wieldable Item")]
    [RequireComponent(typeof(IWieldable))]
    public class WieldableItem : MonoBehaviour, IWieldableItem
    {
        public IItem AttachedItem => m_AttachedItem;
        public DataIdReference<ItemDefinition> DefaultItemDefinition => m_DefaultItemDefinition;

        public IWieldable Wieldable
        {
            get
            {
                if (m_Wieldable == null)
                    m_Wieldable = GetComponent<IWieldable>();

                return m_Wieldable;
            }
        }

        public event UnityAction<IItem> AttachedItemChanged;

        [SerializeField, DataReferenceDetails(HasLabel = true, HasIcon = true)]
        private DataIdReference<ItemDefinition> m_DefaultItemDefinition;

        private IItem m_AttachedItem;
        private IWieldable m_Wieldable;


        public void SetItem(IItem item)
        {
            if (m_AttachedItem == item)
                return;

            m_AttachedItem = item;
            AttachedItemChanged?.Invoke(item);
        }

        private void Awake() => Wieldable.HolsteringEnded += () => SetItem(null);
    }
}
