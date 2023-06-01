using UnityEngine;

namespace IdenticalStudios
{
    public interface IWieldableArmsHandler : ICharacterModule
    {
        Animator Animator { get; }

        void EnableArms(bool enable, float delay = 0f);

        void SetParent(Transform parent);

        void ToggleNextArmSet();
        void SyncWithAnimator(Animator animator);
    }
}
