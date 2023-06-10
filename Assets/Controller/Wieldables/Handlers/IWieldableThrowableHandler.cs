using IdenticalStudios.WieldableSystem;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public interface IWieldableThrowableHandler : ICharacterModule
    {
        int SelectedIndex { get; }

        event UnityAction<Throwable> OnThrow;
        event UnityAction ThrowableCountChanged;
        event UnityAction ThrowableIndexChanged;


        bool TryThrow();
        void SelectNext(bool next);
        int GetThrowableCountAtIndex(int index);
        Throwable GetThrowableAtIndex(int index);
    }
}
