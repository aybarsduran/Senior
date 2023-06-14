using IdenticalStudios.WieldableSystem;
using UnityEngine;

namespace IdenticalStudios.MovementSystem
{
    public class CharacterWieldableVelocityHandler : CharacterBehaviour
    {
        [SerializeField, Range(0.01f, 1f)]
        [Tooltip("How much will the max velocity be affected by the equipped wieldable weight.")]
        private float m_WieldableWeightVelocityMod = 1f;

        private IWeightHandler m_WeightHandler;


        protected override void OnBehaviourEnabled()
        {
            var movement = GetModule<IMovementController>();
            var wieldableController = GetModule<IWieldablesController>();

            movement.VelocityModifier.Add(GetVelocityMod);
            wieldableController.WieldableEquipStopped += OnWieldableChanged;
        }

        protected override void OnBehaviourDisabled()
        {
            var movement = GetModule<IMovementController>();
            var wieldableController = GetModule<IWieldablesController>();

            movement.VelocityModifier.Remove(GetVelocityMod);
            wieldableController.WieldableEquipStopped -= OnWieldableChanged;
        }

        private void OnWieldableChanged(IWieldable wieldable)
        {
            if (wieldable != null)
                m_WeightHandler = wieldable as IWeightHandler;
            else
                m_WeightHandler = null;
        }

        private float GetVelocityMod()
        {
            if (m_WeightHandler == null)
                return 1f;

            float velocityMod = 1f - Mathf.Clamp01(m_WeightHandler.Weight * m_WieldableWeightVelocityMod * 0.016666f);
            return velocityMod;
        }
    }
}
