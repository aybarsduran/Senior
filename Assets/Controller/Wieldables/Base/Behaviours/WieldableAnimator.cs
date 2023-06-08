using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [DisallowMultipleComponent]
    public abstract class WieldableAnimator : WieldableBehaviour
    {
        public abstract Animator Animator { get; }
    }
}