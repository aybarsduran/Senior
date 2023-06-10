using IdenticalStudios.WieldableSystem;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public interface IWieldableHealingHandler : ICharacterModule
    {
        event UnityAction HealsCountChanged;
        event UnityAction<HealingItem> OnHeal;


        bool TryHeal();
        int GetHealsCount();
    }
}
