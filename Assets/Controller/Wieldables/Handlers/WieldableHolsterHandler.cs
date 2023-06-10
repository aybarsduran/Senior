namespace IdenticalStudios.WieldableSystem
{
    public sealed class WieldableHolsterHandler : CharacterBehaviour
    {
        private IWieldablesController m_Controller;
        private IWieldableSelectionHandler m_InventorySelection;


        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_InventorySelection);
            GetModule(out m_Controller);
        }

        public void Holster()
        {
            bool canHolster = !m_Controller.IsEquipping;

            if (!canHolster)
                return;

            if (m_Controller.ActiveWieldable != null)
                m_Controller.TryEquipWieldable(null, 1f);
            else
                m_InventorySelection.SelectAtIndex(m_InventorySelection.SelectedIndex);
        }
    }
}