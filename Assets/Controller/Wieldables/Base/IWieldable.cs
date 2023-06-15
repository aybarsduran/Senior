using UnityEngine.Events;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public interface IWieldable
    {
        ICharacter Character { get; set; }
        IAudioPlayer AudioPlayer { get; set; }
        IAccuracyHandler AccuracyHandler { get; set; }

        bool IsVisible { get; } 
        float EquipDuration { get; }
        float HolsterDuration { get; }

        event UnityAction EquippingStarted;
        event UnityAction HolsteringEnded;


        void OnEquip();
        void OnHolster(float holsterSpeed);
        void SetVisibility(bool visible);

        #region Monobehaviour
        GameObject gameObject { get; }
        Transform transform { get; }
        T GetComponent<T>();
        T GetComponentInChildren<T>(bool inactive = false);
        #endregion
    }
}