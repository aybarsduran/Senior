using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public abstract class FirearmTriggerBehaviour : FirearmAttachmentBehaviour, ITrigger
    {
        public bool IsTriggerHeld { get; protected set; }

        public event UnityAction<float> Shoot;


        public virtual void HoldTrigger()
        {
            if (!IsTriggerHeld)
                TapTrigger();

            IsTriggerHeld = true;
        }

        public virtual void ReleaseTrigger()
        {
            IsTriggerHeld = false;
        }

        protected virtual void TapTrigger() { }
        protected void RaiseShootEvent(float value) => Shoot(value);

        protected virtual void OnEnable() => Firearm?.SetTrigger(this);
        protected virtual void OnDisable() { }
    }
}
